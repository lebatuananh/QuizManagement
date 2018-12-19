using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using QuizManagement.Application.Exams.ViewModel;
using QuizManagement.Data.Entities.Quiz;
using QuizManagement.Data.Enum;
using QuizManagement.Infrastructure.Interfaces;
using QuizManagement.Utilities.Paging;

namespace QuizManagement.Application.Exams
{
    public class ExamService : IExamService
    {
        private readonly IRepository<Exam, int> _examRepository;
        private readonly IRepository<QuestionExamDetail, int> _questiondetailRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ExamService(IRepository<Exam, int> examRepository,
            IRepository<QuestionExamDetail, int> questiondetailRepository, IUnitOfWork unitOfWork)
        {
            _examRepository = examRepository;
            _questiondetailRepository = questiondetailRepository;
            _unitOfWork = unitOfWork;
        }

        public void Create(ExamViewModel examViewModel)
        {
            var exam = Mapper.Map<ExamViewModel, Exam>(examViewModel);
            var examDetails =
                Mapper.Map<List<QuestionExamDetailViewModel>, List<QuestionExamDetail>>(examViewModel
                    .QuestionExamDetailViewModels);
            exam.QuestionExamDetails = examDetails;
            _examRepository.Add(exam);
        }

        public void Update(ExamViewModel examViewModel)
        {
            var examDetails = Mapper.Map<List<QuestionExamDetailViewModel>, List<QuestionExamDetail>>(examViewModel
                    .QuestionExamDetailViewModels);
            var exam = new Exam()
            {
                Id = examViewModel.Id,
                Examiner = examViewModel.Examiner,
                DateCreated = examViewModel.DateCreated,
                DateModified = examViewModel.DateModified,
                ExamName = examViewModel.ExamName,
                QuestionExamDetails = examDetails,
                Status = examViewModel.Status,
                Time = examViewModel.Time

            };
            var newDetails = exam.QuestionExamDetails;
            var addedDetails = newDetails.Where(x => x.Id == 0).ToList();
            var updatedDetails = newDetails.Where(x => x.Id != 0).ToList();
            var existedDetails = _questiondetailRepository.FindAll(x => x.ExamId == examViewModel.Id);
            exam.QuestionExamDetails.Clear();
            foreach (var detail in updatedDetails)
            {
                _questiondetailRepository.Update(detail);
            }

            foreach (var detail in addedDetails)
            {
                _questiondetailRepository.Add(detail);
            }
            _examRepository.Update(exam);
        }

        public PagedResult<ExamViewModel> GetAllPaging(string keyword, int pageIndex, int pageSize)
        {
            var query = _examRepository.FindAll(x => x.Status == Status.Active);
            if (!string.IsNullOrEmpty(keyword))
                query = query.Where(x => x.ExamName.Contains(keyword));
            var totalRow = query.Count();

            var data = query.OrderByDescending(n => n.DateCreated).Skip((pageIndex - 1) * pageIndex).Take(pageSize)
                .ProjectTo<ExamViewModel>().ToList();

            return new PagedResult<ExamViewModel>()
            {
                CurrentPage = pageIndex,
                PageSize = pageSize,
                Results = data,
                RowCount = totalRow
            };
        }

        public ExamViewModel GetById(int examId)
        {
            var exam = _examRepository.FindSingle(x => x.Id == examId);
            var examViewModel = Mapper.Map<Exam, ExamViewModel>(exam);

            var examDetailViewModels = _questiondetailRepository.FindAll(x => x.ExamId == examId)
                .ProjectTo<QuestionExamDetailViewModel>().ToList();
            examViewModel.QuestionExamDetailViewModels = examDetailViewModels;
            return examViewModel;
        }

        public QuestionExamDetailViewModel CreateDetail(QuestionExamDetailViewModel questionExamDetailViewModel)
        {
            var examDetail = Mapper.Map<QuestionExamDetailViewModel, QuestionExamDetail>(questionExamDetailViewModel);
            _questiondetailRepository.Add(examDetail);
            return questionExamDetailViewModel;
        }

        public void DeleteDetail(int examId, int questionId)
        {
            var model = _questiondetailRepository.FindSingle(x => x.QuestionId == questionId && x.ExamId == examId);
            _questiondetailRepository.Remove(model);
        }

        public List<QuestionExamDetailViewModel> GetExamDetails(int examId)
        {
            var model = _questiondetailRepository.FindAll(x => x.ExamId == examId, c => c.Exam, c => c.Question)
                .ProjectTo<QuestionExamDetailViewModel>().ToList();
            return model;
        }

        public void SaveChanges()
        {
            _unitOfWork.Commit();
        }
    }
}
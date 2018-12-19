using System.Collections.Generic;
using System.Linq;
using AutoMapper;
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
        private readonly IRepository<QuestionExamDetail, int> _questionRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ExamService(IRepository<Exam, int> examRepository,
            IRepository<QuestionExamDetail, int> questionRepository, IUnitOfWork unitOfWork)
        {
            _examRepository = examRepository;
            _questionRepository = questionRepository;
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
            var exam = Mapper.Map<ExamViewModel, Exam>(examViewModel);
            var newDetails = exam.QuestionExamDetails;
            var addedDetails = newDetails.Where(x => x.Id == 0).ToList();
            var updatedDetails = newDetails.Where(x => x.Id != 0).ToList();
            var existedDetails = _questionRepository.FindAll(x => x.ExamId == examViewModel.Id);
            exam.QuestionExamDetails.Clear();
            _examRepository.Update(exam);
        }

        public PagedResult<ExamViewModel> GetAllPaging(int? subjectId, string keyword, int pageIndex, int pageSize)
        {
            var query = _examRepository.FindAll(x => x.Status == Status.Active);
            if (!string.IsNullOrEmpty(keyword))
                query = query.Where(x => x.ExamName.Contains(keyword));
            return null;
        }

        public ExamViewModel GetById(int examId)
        {
            throw new System.NotImplementedException();
        }

        public QuestionExamDetailViewModel CreateDetail(QuestionExamDetailViewModel questionExamDetailViewModel)
        {
            throw new System.NotImplementedException();
        }

        public void DeleteDetail(int examId, int questionId)
        {
            throw new System.NotImplementedException();
        }

        public List<QuestionExamDetailViewModel> GetExamDetails(int examId)
        {
            throw new System.NotImplementedException();
        }

        public void SaveChanges()
        {
            throw new System.NotImplementedException();
        }
    }
}
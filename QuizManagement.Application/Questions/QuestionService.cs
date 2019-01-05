using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using OfficeOpenXml;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using QuizManagement.Application.Questions.ViewModel;
using QuizManagement.Data.Entities.Quiz;
using QuizManagement.Data.Enum;
using QuizManagement.Infrastructure.Interfaces;
using QuizManagement.Utilities.Paging;

namespace QuizManagement.Application.Questions
{
    public class QuestionService : IQuestionService
    {
        private readonly IRepository<Question, int> _questionRepository;
        private readonly IUnitOfWork _unitOfWork;

        public QuestionService(IRepository<Question, int> questionRepository, IUnitOfWork unitOfWork)
        {
            _questionRepository = questionRepository;
            _unitOfWork = unitOfWork;
        }

        public void Create(QuestionViewModel questionViewModel)
        {
            var question = Mapper.Map<QuestionViewModel, Question>(questionViewModel);
            _questionRepository.Add(question);
        }

        public void Delete(int id)
        {
            _questionRepository.Remove(id);
        }

        public List<QuestionViewModel> GetAll()
        {
            return _questionRepository.FindAll().ProjectTo<QuestionViewModel>().ToList();
        }

        public List<QuestionViewModel> GetBySubject(int subjectId)
        {
            return _questionRepository.FindAll(x => x.SubjectId == subjectId).ProjectTo<QuestionViewModel>().ToList();
        }

        public PagedResult<QuestionViewModel> GetAllPaging(int? chapterId, int? subjectId, string keyword, int page,
            int pageSize)
        {
            var query = _questionRepository.FindAll(x => x.Status == Status.Active);
            if (!string.IsNullOrEmpty(keyword))
                query = query.Where(x => x.QuestionName.Contains(keyword));
            if (chapterId.HasValue || subjectId.HasValue)
                query = query.Where(x => x.SubjectId == subjectId && x.ChapterId == chapterId);
            int totalRow = query.Count();
            query = query.OrderBy(n => n.ScoreQuestion).Skip((page - 1) * pageSize).Take(pageSize);
            var data = query.ProjectTo<QuestionViewModel>().ToList();
            var paginationSet = new PagedResult<QuestionViewModel>()
            {
                Results = data,
                CurrentPage = page,
                RowCount = totalRow,
                PageSize = pageSize
            };

            return paginationSet;
        }

        public QuestionViewModel GetById(int id)
        {
            var query = _questionRepository.FindById(id);
            var model = Mapper.Map<Question, QuestionViewModel>(query);
            return model;
        }

        public void SaveChanges()
        {
            _unitOfWork.Commit();
        }

        public void ImportExcel(string filePath, int chapterId, int subjectId)
        {
            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                ExcelWorksheet workSheet = package.Workbook.Worksheets[1];
                Question question;
                for (int i = workSheet.Dimension.Start.Row + 1; i <= workSheet.Dimension.End.Row; i++)
                {
                    question = new Question();
                    question.ChapterId = chapterId;
                    question.SubjectId = subjectId;

                    question.QuestionName = workSheet.Cells[i, 1].Value.ToString();

                    question.Option1 = workSheet.Cells[i, 2].Value.ToString();
                    question.Option2 = workSheet.Cells[i, 3].Value.ToString();
                    question.Option3 = workSheet.Cells[i, 4].Value.ToString();
                    question.Option4 = workSheet.Cells[i, 5].Value.ToString();
                    question.Answer = workSheet.Cells[i, 6].Value.ToString();
                    int.TryParse(workSheet.Cells[i, 7].Value.ToString(), out var questionScore);
                    question.ScoreQuestion = questionScore;

                    question.Status = Status.Active;

                    _questionRepository.Add(question);
                }
            }
        }

        public void Update(QuestionViewModel questionViewModel)
        {
            var question = Mapper.Map<QuestionViewModel, Question>(questionViewModel);
            _questionRepository.Update(question);
        }
    }
}
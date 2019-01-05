using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using QuizManagement.Application.Exams.ViewModel;
using QuizManagement.Application.Questions;
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
        private readonly IQuestionService _questionService;

        public ExamService(IRepository<Exam, int> examRepository,
            IRepository<QuestionExamDetail, int> questiondetailRepository, IUnitOfWork unitOfWork
            , IQuestionService questionService)
        {
            _examRepository = examRepository;
            _questiondetailRepository = questiondetailRepository;
            _unitOfWork = unitOfWork;
            _questionService = questionService;
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

        public ExamViewModel CreateRandom(RandomExamViewModel model)
        {
            var allQuestions = _questionService.GetBySubject(model.SubjectId).ToArray();
            bool[] checkQuestion = new bool[allQuestions.Length];
            if (allQuestions.Length == 0)
            {
                throw new Exception("Question not found");
            }
            if (allQuestions.Length < model.QuestionsNumber)
            {
                throw new Exception("Too many questions");
            }

            ExamViewModel examVm = new ExamViewModel
            {
                ExamName = model.ExamName,
                Time = model.Time,
                Examiner = model.Examiner,
                DateCreated = model.DateCreated,
                DateModified = model.DateModified,
                Status = model.Status,
                QuestionExamDetailViewModels = new List<QuestionExamDetailViewModel>()
            };

            Random rand = new Random();
            int j = -1;
            for (int i = 0; i < model.QuestionsNumber; i++)
            {
                while (true)
                {
                    j = rand.Next(allQuestions.Length);
                    if (!checkQuestion[j])
                    {
                        checkQuestion[j] = !checkQuestion[j];
                        break;
                    }
                }
                var question = new QuestionExamDetailViewModel
                {
                    ExamId = examVm.Id,
                    QuestionId = allQuestions[j].Id
                };
                examVm.QuestionExamDetailViewModels.Add(question);
            }

            return examVm;
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

        public FileStream ExportToWord(int examId)
        {
            var model = this.GetById(examId);
            //  Todo Export here
            var filePath = Path.GetTempFileName();
            using (var wordprocessingDocument = WordprocessingDocument.Create(filePath, WordprocessingDocumentType.Document))
            {
                // Add a main document part. 
                MainDocumentPart mainPart = wordprocessingDocument.AddMainDocumentPart();

                // Create the document structure and add some text.
                mainPart.Document = new Document();
                Body body = mainPart.Document.AppendChild(new Body());
                Paragraph para = body.AppendChild(new Paragraph());
                Run run = para.AppendChild(new Run());
                run.AppendChild(new Text(model.ExamName));
                run.AppendChild(new Break());
                run.AppendChild(new Text("Time: " + model.Time.ToString()));
                run.AppendChild(new Break());
                run.AppendChild(new Text("Examiner: " + model.Examiner));
                run.AppendChild(new Break());
                int count = 0;
                foreach (var item in model.QuestionExamDetailViewModels)
                {
                    var question = _questionService.GetById(item.QuestionId);
                    run.AppendChild(new Break());
                    run.AppendChild(new Text(++count + ". " + question.QuestionName));
                    run.AppendChild(new Break());
                    run.AppendChild(new Text("A." + question.Option1));
                    run.AppendChild(new Break());
                    run.AppendChild(new Text("B." + question.Option2));
                    run.AppendChild(new Break());
                    run.AppendChild(new Text("C." + question.Option3));
                    run.AppendChild(new Break());
                    run.AppendChild(new Text("D." + question.Option4));
                    run.AppendChild(new Break());
                }
            }
            var fs = System.IO.File.OpenRead(filePath);
            return fs;
        }

        public void SaveChanges()
        {
            _unitOfWork.Commit();
        }
    }
}
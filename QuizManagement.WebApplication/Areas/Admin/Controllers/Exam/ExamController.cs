using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using QuizManagement.Application.Exams;
using QuizManagement.Application.Exams.ViewModel;
using QuizManagement.Application.Questions;
using QuizManagement.Data.Entities.System;
using QuizManagement.WebApplication.Areas.Admin.Controllers.Base;
using QuizManagement.WebApplication.Areas.Admin.Models;

namespace QuizManagement.WebApplication.Areas.Admin.Controllers.Exam
{
    public class ExamController : BaseController
    {
        private readonly IExamService _examService;
        private readonly IAuthorizationService _authorizationService;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IQuestionService _questionService;

        public ExamController(IExamService examService, IAuthorizationService authorizationService,
            SignInManager<AppUser> signInManager, IQuestionService questionService)
        {
            _examService = examService;
            _authorizationService = authorizationService;
            _signInManager = signInManager;
            _questionService = questionService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetById(int id)
        {
            var model = _examService.GetById(id);
            return new OkObjectResult(model);
        }

        [HttpGet]
        public IActionResult GetWordDoc(int id)
        {
            var model = _examService.GetById(id);
            //  Todo Export here
            var filePath = Path.GetTempFileName();
            var cd = new System.Net.Mime.ContentDisposition
            {
                FileName = "ouput.docx",
                Inline = true,
            };
            Response.Headers.Add("Content-Disposition", cd.ToString());
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
            return File(fs, "application/ms-word", "output.docx");
        }

        [HttpGet]
        public IActionResult GetAllPaging(string keyword, int page, int pageSize)
        {
            var model = _examService.GetAllPaging(keyword, page, pageSize);
            return new OkObjectResult(model);
        }

        [HttpPost]
        public IActionResult SaveEntity(ExamViewModel examVm)
        {
            if (!ModelState.IsValid)
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                return new BadRequestObjectResult(allErrors);
            }

            if (examVm.Id == 0)
            {
                _examService.Create(examVm);
            }
            else
            {
                _examService.Update(examVm);
            }

            _examService.SaveChanges();
            return new OkObjectResult(examVm);
        }

        [HttpPost]
        public IActionResult AddRandomQuestions(RandomExamModel model)
        {
            if (!ModelState.IsValid)
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                return new BadRequestObjectResult(allErrors);
            }

            var allQuestions = _questionService.GetBySubject(model.SubjectId).ToArray();
            bool[] checkQuestion = new bool[allQuestions.Length];
            if (allQuestions.Length == 0)
            {
                return new BadRequestObjectResult("Question not found");
            }
            if (allQuestions.Length < model.QuestionsNumber)
            {
                return new BadRequestObjectResult("Too many questions");
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

            if (examVm.Id == 0)
            {
                _examService.Create(examVm);
            }
            else
            {
                _examService.Update(examVm);
            }

            _examService.SaveChanges();
            return new OkObjectResult(examVm);
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using QuizManagement.Application.Chapters;
using QuizManagement.Application.Questions;
using QuizManagement.Application.Questions.ViewModel;
using QuizManagement.Application.Subjects;
using QuizManagement.Data.Entities.System;
using QuizManagement.WebApplication.Areas.Admin.Controllers.Base;

namespace QuizManagement.WebApplication.Areas.Admin.Controllers.Question
{
    public class QuestionController : BaseController
    {
        private readonly IChapterService _chapterService;
        private readonly ISubjectService _subjectService;
        private readonly IQuestionService _questionService;
        private readonly IAuthorizationService _authorizationService;
        private readonly SignInManager<AppUser> _signInManager;

        public QuestionController(IChapterService chapterService, ISubjectService subjectService,
            IQuestionService questionService, IAuthorizationService authorizationService,
            SignInManager<AppUser> signInManager)
        {
            _chapterService = chapterService;
            _subjectService = subjectService;
            _questionService = questionService;
            _authorizationService = authorizationService;
            _signInManager = signInManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetAllPaging(int? chapterId, int? subjectId, string keyword, int page, int pageSize)
        {
            var model = _questionService.GetAllPaging(chapterId, subjectId, keyword, page, pageSize);
            return new OkObjectResult(model);
        }

        [HttpPost]
        public IActionResult SaveEntity(QuestionViewModel questionViewModel)
        {
            if (!ModelState.IsValid)
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                return new BadRequestObjectResult(allErrors);
            }
            else
            {
                try
                {
                    if (questionViewModel.Id == 0)
                    {
                        _questionService.Create(questionViewModel);
                    }
                    else
                    {
                        _questionService.Update(questionViewModel);
                    }

                    _questionService.SaveChanges();
                }
                catch (System.Exception e)
                {
                    return new BadRequestResult();
                }

                return new OkObjectResult(_questionService);
            }
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            _questionService.Delete(id);
            _questionService.SaveChanges();
            return new OkResult();
        }

        [HttpGet]
        public IActionResult GetById(int id)
        {
            var model = _questionService.GetById(id);
            return new OkObjectResult(model);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var model = _questionService.GetAll();
            return new OkObjectResult(model);
        }
    }
}
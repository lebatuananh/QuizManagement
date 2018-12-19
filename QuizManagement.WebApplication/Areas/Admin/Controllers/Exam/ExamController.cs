using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using QuizManagement.Application.Exams;
using QuizManagement.Application.Exams.ViewModel;
using QuizManagement.Data.Entities.System;
using QuizManagement.WebApplication.Areas.Admin.Controllers.Base;

namespace QuizManagement.WebApplication.Areas.Admin.Controllers.Exam
{
    public class ExamController : BaseController
    {
        private readonly IExamService _examService;
        private readonly IAuthorizationService _authorizationService;
        private readonly SignInManager<AppUser> _signInManager;

        public ExamController(IExamService examService, IAuthorizationService authorizationService,
            SignInManager<AppUser> signInManager)
        {
            _examService = examService;
            _authorizationService = authorizationService;
            _signInManager = signInManager;
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
    }
}
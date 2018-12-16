using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using QuizManagement.Application.Subjects;
using QuizManagement.Application.Subjects.ViewModel;
using QuizManagement.WebApplication.Areas.Admin.Controllers.Base;

namespace QuizManagement.WebApplication.Areas.Admin.Controllers.Subject
{
    public class SubjectController:BaseController
    {
        private readonly ISubjectService _subjectService;

        public SubjectController(ISubjectService subjectService)
        {
            _subjectService = subjectService;
        }
        
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetById(int id)
        {
            return new OkObjectResult(_subjectService.GetById(id));
        }

        [HttpGet]
        public IActionResult GetAllPaging(string keyword, int page, int pageSize)
        {
            return new OkObjectResult(_subjectService.GetAllPaging(keyword, page, pageSize));
        }

        [HttpPost]
        public IActionResult SaveEntity(SubjectViewModel subjectViewModel)
        {
            if (!ModelState.IsValid)
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                return new BadRequestObjectResult(allErrors);
            }

            if (subjectViewModel.Id == 0)
            {
                _subjectService.Add(subjectViewModel);
            }
            else
            {
                _subjectService.Update(subjectViewModel);
            }

            _subjectService.SaveChanges();
            return new OkObjectResult(subjectViewModel);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            _subjectService.Delete(id);
            _subjectService.SaveChanges();
            return new OkObjectResult(id);
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using QuizManagement.Application.Chapters;
using QuizManagement.Application.Chapters.ViewModel;
using QuizManagement.WebApplication.Areas.Admin.Controllers.Base;

namespace QuizManagement.WebApplication.Areas.Admin.Controllers.Chapter
{
    public class ChapterController : BaseController
    {
        private readonly IChapterService _chapterService;

        public ChapterController(IChapterService chapterService)
        {
            _chapterService = chapterService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetById(int id)
        {
            return new OkObjectResult(_chapterService.GetById(id));
        }

        [HttpGet]
        public IActionResult GetAllPaging(string keyword, int page, int pageSize)
        {
            return new OkObjectResult(_chapterService.GetAllPaging(keyword, page, pageSize));
        }

        [HttpPost]
        public IActionResult SaveEntity(ChapterViewModel chapterViewModel)
        {
            if (!ModelState.IsValid)
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                return new BadRequestObjectResult(allErrors);
            }

            if (chapterViewModel.Id == 0)
            {
                _chapterService.Add(chapterViewModel);
            }
            else
            {
                _chapterService.Update(chapterViewModel);
            }

            _chapterService.SaveChanges();
            return new OkObjectResult(chapterViewModel);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            _chapterService.Delete(id);
            _chapterService.SaveChanges();
            return new OkObjectResult(id);
        }
    }
}
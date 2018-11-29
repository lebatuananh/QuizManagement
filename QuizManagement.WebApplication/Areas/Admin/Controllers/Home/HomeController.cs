using Microsoft.AspNetCore.Mvc;
using QuizManagement.WebApplication.Areas.Admin.Controllers.Base;
using QuizManagement.WebApplication.Extensions;

namespace QuizManagement.WebApplication.Areas.Admin.Controllers.Home
{
    public class HomeController : BaseController
    {
        public IActionResult Index()
        {
            var email = User.GetSpecificClaim("Email");

            return View();
        }
    }
}
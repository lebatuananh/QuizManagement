using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using QuizManagement.Data.Entities.System;
using QuizManagement.WebApplication.Areas.Admin.Controllers.Base;

namespace QuizManagement.WebApplication.Areas.Admin.Controllers.Account
{
    public class AccountController : BaseController
    {
        private readonly SignInManager<AppUser> _signInManager;

        public AccountController(SignInManager<AppUser> signInManager)
        {
            this._signInManager = signInManager;
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Redirect("/Admin/Login/Index");
        }
    }
}
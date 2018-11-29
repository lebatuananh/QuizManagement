using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using QuizManagement.Data.Entities.System;
using QuizManagement.Utilities.Paging;
using QuizManagement.WebApplication.Areas.Admin.Controllers.Base;
using QuizManagement.WebApplication.Areas.Admin.Models.AccoutViewModel;

namespace QuizManagement.WebApplication.Areas.Admin.Controllers.Login
{
    [AllowAnonymous]
    public class LoginController : BaseController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ILogger _logger;

        public LoginController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,
            ILogger<LoginController> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Authen(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");
                    return new OkObjectResult(new GenericResult(true));
                }

                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out");
                    return new ObjectResult(new GenericResult(false, "Tài khoản này đã bị khóa"));
                }
                else
                {
                    return new ObjectResult(new GenericResult(false, "Đăng nhập thất bại"));
                }
            }
            // If we got this far, something failed, redisplay form
            return new ObjectResult(new GenericResult(false, model));
        }
    }
}
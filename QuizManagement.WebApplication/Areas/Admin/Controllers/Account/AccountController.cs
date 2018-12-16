using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using QuizManagement.Application.Users;
using QuizManagement.Application.Users.ViewModel;
using QuizManagement.Data.Entities.System;
using QuizManagement.Utilities.Paging;
using QuizManagement.WebApplication.Areas.Admin.Controllers.Base;
using QuizManagement.WebApplication.Areas.Admin.Models.AccoutViewModel;
using QuizManagement.WebApplication.Extensions;

namespace QuizManagement.WebApplication.Areas.Admin.Controllers.Account
{
    public class AccountController : BaseController
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IUserService _userService;

        public AccountController(SignInManager<AppUser> signInManager, IUserService userService)
        {
            _signInManager = signInManager;
            _userService = userService;
        }
        
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Account()
        {
            var id = User.GetSpecificClaim("UserId");
            var model = await _userService.GetByIdAsync(id);
            return new OkObjectResult(model);
        }

        [HttpPost]
        public async Task<IActionResult> Update(AppUserViewModel userVm)
        {
            if (!ModelState.IsValid)
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                return new BadRequestObjectResult(allErrors);
            }
            else
            {
                var isValid = await _userService.UpdateAccount(userVm);
                if (isValid == false)
                    return new OkObjectResult(new GenericResult(false, userVm));

                return new OkObjectResult(new GenericResult(true, userVm));
            }
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(PasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                return new BadRequestObjectResult(allErrors);
            }
            else
            {
                var userId = User.GetUserId();

                var isValid =
                    await _userService.ChangePassword(userId.ToString(), model.CurrentPassword, model.Password);

                if (isValid)
                {
                    await _signInManager.SignOutAsync();
                    return new OkObjectResult(new GenericResult(true));
                }
                else
                {
                    return new OkObjectResult(new GenericResult(false));
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Redirect("/Admin/Login/Index");
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using QuizManagement.Application.Users;
using QuizManagement.Application.Users.ViewModel;
using QuizManagement.Data.Entities.System;
using QuizManagement.Utilities.Constants;
using QuizManagement.Utilities.Paging;
using QuizManagement.WebApplication.Areas.Admin.Controllers.Base;
using QuizManagement.WebApplication.Authorization;

namespace QuizManagement.WebApplication.Areas.Admin.Controllers.User
{
    public class UserController : BaseController
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IUserService _userService;
        private readonly IAuthorizationService _authorizationService;

        public UserController(SignInManager<AppUser> signInManager, IUserService userService,IAuthorizationService authorizationService)
        {
            _signInManager = signInManager;
            _userService = userService;
            _authorizationService = authorizationService;
        }

        public async Task<IActionResult> Index()
        {
            var result = await _authorizationService.AuthorizeAsync(User, "USER", Operations.Read);

            if (result.Succeeded == false)
            {
                await _signInManager.SignOutAsync();
                return new RedirectResult("/Admin/Login/Index");
            }

            return View();
        }

        public IActionResult GetAll()
        {
            var model = _userService.GetAllAsync();

            return new OkObjectResult(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetById(string id)
        {
            var model = await _userService.GetByIdAsync(id);

            return new OkObjectResult(model);
        }

        [HttpGet]
        public IActionResult GetAllPaging(string keyword, int page, int pageSize)
        {
            var model = _userService.GetAllPagingAsync(keyword, page, pageSize);
            return new OkObjectResult(model);
        }

        [HttpPost]
        public async Task<IActionResult> SaveEntity(AppUserViewModel userVm)
        {
            if (!ModelState.IsValid)
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                return new BadRequestObjectResult(allErrors);
            }
            else
            {
                if (userVm.Id == null)
                {
                    var isValid = await _userService.AddAsync(userVm);
                    if (isValid == false)
                        return new OkObjectResult(new GenericResult(false, userVm));
                }
                else
                {
                    var isValid = await _userService.UpdateAsync(userVm);
                    if (isValid == false)
                        return new OkObjectResult(new GenericResult(false, userVm));
                }

                return new OkObjectResult(new GenericResult(true, userVm));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }
            else
            {
                await _userService.DeleteAsync(id);

                return new OkObjectResult(id);
            }
        }
        
        [HttpPost]
        public async Task<IActionResult> ResetPassword(string userId)
        {

            if (string.IsNullOrEmpty(userId))
            {
                return new BadRequestResult();

            }

            var isValid = await _userService.ResetPassword(userId, CommonConstants.DefaultPassword);

            if (isValid)
            {
                return new OkObjectResult(new GenericResult(true));
            }
            else
            {
                return new OkObjectResult(new GenericResult(false));
            }
        }
    }
}
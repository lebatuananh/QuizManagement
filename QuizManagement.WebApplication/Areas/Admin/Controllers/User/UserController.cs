using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using QuizManagement.Application.Users;
using QuizManagement.Application.Users.ViewModel;
using QuizManagement.Data.Entities.System;
using QuizManagement.Data.Enum;
using QuizManagement.Utilities.Paging;
using QuizManagement.WebApplication.Areas.Admin.Controllers.Base;
using QuizManagement.WebApplication.Extensions;

namespace QuizManagement.WebApplication.Areas.Admin.Controllers.User
{
    public class UserController : BaseController
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IUserService _userService;

        public UserController(SignInManager<AppUser> signInManager, IUserService userService)
        {
            _signInManager = signInManager;
            _userService = userService;
        }

        public IActionResult Index()
        {
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
    }
}
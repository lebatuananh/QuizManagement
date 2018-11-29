using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using QuizManagement.Application.Permissions.ViewModel;
using QuizManagement.Application.Roles;
using QuizManagement.Application.Roles.ViewModel;
using QuizManagement.Data.Entities.System;
using QuizManagement.Utilities.Paging;
using QuizManagement.WebApplication.Areas.Admin.Controllers.Base;

namespace QuizManagement.WebApplication.Areas.Admin.Controllers.Role
{
    public class RoleController : BaseController
    {
        private readonly IRoleService _roleService;
        private readonly SignInManager<AppUser> _signInManager;

        public RoleController(IRoleService roleService, SignInManager<AppUser> signInManager)
        {
            _roleService = roleService;
            _signInManager = signInManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> GetAll()
        {
            var model = await _roleService.GetAllAsync();

            return new OkObjectResult(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetById(Guid id)
        {
            var model = await _roleService.GetById(id);

            return new OkObjectResult(model);
        }

        [HttpGet]
        public IActionResult GetAllPaging(string keyword, int page, int pageSize)
        {
            var model = _roleService.GetAllPagingAsync(keyword, page, pageSize);
            return new OkObjectResult(model);
        }

        [HttpPost]
        public async Task<IActionResult> SaveEntity(AppRoleViewModel roleVm)
        {
            if (!ModelState.IsValid)
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                return new BadRequestObjectResult(allErrors);
            }

            if (!roleVm.Id.HasValue)
            {
                var result = await _roleService.AddAsync(roleVm);

                if (result == false)
                {
                    return new OkObjectResult(new GenericResult(false, roleVm));
                }
            }
            else
            {
                var result = await _roleService.UpdateAsync(roleVm);

                if (result == false)
                {
                    return new OkObjectResult(new GenericResult(false, roleVm));
                }
            }

            return new OkObjectResult(new GenericResult(true, roleVm));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            await _roleService.DeleteAsync(id);
            return new OkObjectResult(id);
        }

        [HttpPost]
        public IActionResult ListAllFunction(Guid roleId)
        {
            var functions = _roleService.GetListFunctionWithRole(roleId);
            return new OkObjectResult(functions);
        }

        [HttpPost]
        public IActionResult SavePermission(List<PermissionViewModel> listPermmission, Guid roleId)
        {
            _roleService.SavePermission(listPermmission, roleId);
            return new OkResult();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using QuizManagement.Application.Permissions.ViewModel;
using QuizManagement.Application.Roles.ViewModel;
using QuizManagement.Utilities.Paging;

namespace QuizManagement.Application.Roles
{
    public interface IRoleService
    {
        Task<bool> AddAsync(AppRoleViewModel userVm);

        Task DeleteAsync(Guid id);

        Task<List<AppRoleViewModel>> GetAllAsync();

        PagedResult<AppRoleViewModel> GetAllPagingAsync(string keyword, int page, int pageSize);

        Task<AppRoleViewModel> GetById(Guid id);

        Task<bool> UpdateAsync(AppRoleViewModel userVm);

        List<PermissionViewModel> GetListFunctionWithRole(Guid roleId);

        void SavePermission(List<PermissionViewModel> permissions, Guid roleId);

        Task<bool> CheckPermission(string functionId, string action, string[] roles);

        Task<AppRoleViewModel> GetByName(string role);

        List<PermissionViewModel> GetListFunctionMenuWithRole(Guid roleId);
    }
}
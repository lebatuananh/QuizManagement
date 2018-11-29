using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using QuizManagement.Application.Roles.ViewModel;
using QuizManagement.Data.Entities.System;
using QuizManagement.Infrastructure.Interfaces;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using QuizManagement.Application.Permissions.ViewModel;
using QuizManagement.Utilities.Paging;

namespace QuizManagement.Application.Roles
{
    public class RoleService : IRoleService
    {
        private RoleManager<AppRole> _roleManager;
        private IRepository<Function, string> _functionRepository;
        private IRepository<Permission, int> _permissionRepository;
        private IUnitOfWork _unitOfWork;

        public RoleService(RoleManager<AppRole> roleManager, IRepository<Function, string> functionRepository,
            IRepository<Permission, int> permissionRepository, IUnitOfWork unitOfWork)
        {
            _roleManager = roleManager;
            _functionRepository = functionRepository;
            _permissionRepository = permissionRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> AddAsync(AppRoleViewModel roleVm)
        {
            var findRole = await _roleManager.FindByNameAsync(roleVm.Name);

            if (findRole != null)
                return false;

            var role = new AppRole()
            {
                Name = roleVm.Name,
                Description = roleVm.Description
            };
            var result = await _roleManager.CreateAsync(role);
            _unitOfWork.Commit();
            return result.Succeeded;
        }

        public Task<bool> CheckPermission(string functionId, string action, string[] roles)
        {
            var functions = _functionRepository.FindAll();
            var permissions = _permissionRepository.FindAll();
            var query = from f in functions
                join p in permissions on f.Id equals p.FunctionId
                join r in _roleManager.Roles on p.RoleId equals r.Id
                where roles.Contains(r.Name) && f.Id == functionId
                                             && ((p.CanCreate == true && action == "Create")
                                                 || (p.CanUpdate == true && action == "Update")
                                                 || (p.CanDelete == true && action == "Delete")
                                                 || (p.CanRead == true && action == "Read"))
                select p;
            return query.AnyAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var role = await _roleManager.FindByIdAsync(id.ToString());
            await _roleManager.DeleteAsync(role);
        }

        public async Task<List<AppRoleViewModel>> GetAllAsync()
        {
            return await _roleManager.Roles.ProjectTo<AppRoleViewModel>().ToListAsync();
        }

        public PagedResult<AppRoleViewModel> GetAllPagingAsync(string keyword, int page, int pageSize)
        {
            var query = _roleManager.Roles;
            if (!string.IsNullOrEmpty(keyword))
                query = query.Where(x => x.Name.Contains(keyword)
                                         || x.Description.Contains(keyword));

            int totalRow = query.Count();
            query = query.Skip((page - 1) * pageSize)
                .Take(pageSize);

            var data = query.ProjectTo<AppRoleViewModel>().ToList();
            var paginationSet = new PagedResult<AppRoleViewModel>()
            {
                Results = data,
                CurrentPage = page,
                RowCount = totalRow,
                PageSize = pageSize
            };

            return paginationSet;
        }

        public async Task<AppRoleViewModel> GetById(Guid id)
        {
            var role = await _roleManager.FindByIdAsync(id.ToString());
            return Mapper.Map<AppRole, AppRoleViewModel>(role);
        }

        public async Task<AppRoleViewModel> GetByName(string name)
        {
            var role = await _roleManager.FindByNameAsync(name);
            return Mapper.Map<AppRole, AppRoleViewModel>(role);
        }

        public List<PermissionViewModel> GetListFunctionWithRole(Guid roleId)
        {
            var functions = _functionRepository.FindAll();
            var permissions = _permissionRepository.FindAll();

            var query = from f in functions
                join p in permissions on f.Id equals p.FunctionId into fp
                from p in fp.DefaultIfEmpty()
                where p != null && p.RoleId == roleId
                select new PermissionViewModel()
                {
                    RoleId = roleId,
                    FunctionId = f.Id,
                    CanCreate = p != null ? p.CanCreate : false,
                    CanDelete = p != null ? p.CanDelete : false,
                    CanRead = p != null ? p.CanRead : false,
                    CanUpdate = p != null ? p.CanUpdate : false
                };
            return query.ToList();
        }

        public List<PermissionViewModel> GetListFunctionMenuWithRole(Guid roleId)
        {
            var functions = _functionRepository.FindAll();
            var permissions = _permissionRepository.FindAll();

            var query = from f in functions
                join p in permissions on f.Id equals p.FunctionId into fp
                from p in fp.DefaultIfEmpty()
                where p != null && p.RoleId == roleId &&
                      (
                          p.CanCreate == true
                          || p.CanDelete == true
                          || p.CanRead == true
                          || p.CanUpdate == true
                      )
                select new PermissionViewModel()
                {
                    RoleId = roleId,
                    FunctionId = f.Id,
                    CanCreate = p != null ? p.CanCreate : false,
                    CanDelete = p != null ? p.CanDelete : false,
                    CanRead = p != null ? p.CanRead : false,
                    CanUpdate = p != null ? p.CanUpdate : false
                };
            return query.ToList();
        }

        public void SavePermission(List<PermissionViewModel> permissionVms, Guid roleId)
        {
            var permissions = Mapper.Map<List<PermissionViewModel>, List<Permission>>(permissionVms);
            var oldPermission = _permissionRepository.FindAll().Where(x => x.RoleId == roleId).ToList();
            if (oldPermission.Count > 0)
            {
                _permissionRepository.RemoveMultiple(oldPermission);
            }

            foreach (var permission in permissions)
            {
                _permissionRepository.Add(permission);
            }

            _unitOfWork.Commit();
        }

        public async Task<bool> UpdateAsync(AppRoleViewModel roleVm)
        {
            var role = await _roleManager.FindByIdAsync(roleVm.Id.ToString());
            var findByName = await _roleManager.FindByNameAsync(roleVm.Name);

            if (!role.Name.Equals(role.Name) && findByName != null)
            {
                return false;
            }

            role.Description = roleVm.Description;
            role.Name = roleVm.Name;
            await _roleManager.UpdateAsync(role);
            _unitOfWork.Commit();
            return true;
        }
    }
}
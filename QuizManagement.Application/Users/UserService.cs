using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QuizManagement.Application.Users.ViewModel;
using QuizManagement.Data.Entities.System;
using QuizManagement.Infrastructure.Interfaces;
using QuizManagement.Utilities.Paging;

namespace QuizManagement.Application.Users
{
    public class UserService : IUserService
    {
        private readonly UserManager<AppUser> _userManager;
        private IRepository<Function, string> _functionRepository;
        private IRepository<Permission, int> _permissionRepository;
        private RoleManager<AppRole> _roleManager;

        public UserService(UserManager<AppUser> userManager, IRepository<Function, string> functionRepository,
            IRepository<Permission, int> permissionRepository, RoleManager<AppRole> roleManager)
        {
            _userManager = userManager;
            _functionRepository = functionRepository;
            _permissionRepository = permissionRepository;
            _roleManager = roleManager;
        }

        public async Task<bool> AddAsync(AppUserViewModel viewModel)
        {
            var findByEmail = await _userManager.FindByEmailAsync(viewModel.Email);
            var findByUsername = await _userManager.FindByNameAsync(viewModel.UserName);

            if (findByEmail != null || findByUsername != null)
            {
                return false;
            }

            var user = new AppUser()
            {
                UserName = viewModel.UserName,
                Avatar = viewModel.Avatar,
                FullName = viewModel.FullName,
                DateCreated = DateTime.Now,
                DateModified = DateTime.Now,
                Status = viewModel.Status,
                Email = viewModel.Email,
                PhoneNumber = viewModel.PhoneNumber
            };


            var result = await _userManager.CreateAsync(user, viewModel.Password);

            if (result.Succeeded && viewModel.Roles.Count > 0)
            {
                var appUser = await _userManager.FindByNameAsync(user.UserName);

                if (appUser != null)
                    await _userManager.AddToRolesAsync(appUser, viewModel.Roles);
            }

            return true;
        }

        public async Task DeleteAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            await _userManager.DeleteAsync(user);
        }

        public async Task<List<AppUserViewModel>> GetAllAsync()
        {
            return await _userManager.Users.ProjectTo<AppUserViewModel>().ToListAsync();
        }

        public PagedResult<AppUserViewModel> GetAllPagingAsync(string keyword, int page, int pageSize)
        {
            var query = _userManager.Users;

            if (!string.IsNullOrEmpty(keyword))
                query = query.Where(n =>
                    n.UserName.Contains(keyword) || n.Email.Contains(keyword) || n.FullName.Contains(keyword));

            var totalRow = query.Count();

            query = query.Skip((page - 1) * pageSize).Take(pageSize);

            var data = query.Select(n => new AppUserViewModel()
            {
                UserName = n.UserName,
                Avatar = n.Avatar,
                FullName = n.FullName,
                BirthDay = n.BirthDay.ToString(),
                Email = n.Email,
                Id = n.Id,
                PhoneNumber = n.PhoneNumber,
                Status = n.Status,
                DateCreated = n.DateCreated
            }).ToList();

            var paginationSet = new PagedResult<AppUserViewModel>
            {
                RowCount = totalRow,
                Results = data,
                PageSize = pageSize,
                CurrentPage = page
            };

            return paginationSet;
        }

        public async Task<AppUserViewModel> GetByIdAsync(string id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);

                var userRole = await _userManager.GetRolesAsync(user);

                var model = Mapper.Map<AppUser, AppUserViewModel>(user);

                model.Roles = userRole.ToList();

                return model;
            }
            catch (Exception e)
            {
                return null;
                throw;
            }
        }

        public async Task<bool> UpdateAsync(AppUserViewModel userVm)
        {
            var user = await _userManager.FindByIdAsync(userVm.Id.ToString());
            var findByEmail = await _userManager.FindByEmailAsync(userVm.Email);

            if (!user.Email.Equals(userVm.Email) && findByEmail != null)
                return false;

            //remove current roles in db

            var currentRoles = await _userManager.GetRolesAsync(user);

            var result = await _userManager.AddToRolesAsync(user, userVm.Roles.Except(currentRoles).ToArray());

            if (result.Succeeded)
            {
                string[] needRemoveRoles = currentRoles.Except(userVm.Roles).ToArray();
                await _userManager.RemoveFromRolesAsync(user, needRemoveRoles);

                //Update user detail

                user.FullName = userVm.FullName;
                user.Email = userVm.Email;
                user.Status = userVm.Status;
                user.PhoneNumber = userVm.PhoneNumber;
                user.DateModified = DateTime.Now;
                await _userManager.UpdateAsync(user);
                return true;
            }

            return false;
        }
    }
}
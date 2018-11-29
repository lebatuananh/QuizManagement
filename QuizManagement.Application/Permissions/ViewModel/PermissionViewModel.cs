using System;
using QuizManagement.Application.Functions.ViewModel;
using QuizManagement.Application.Roles.ViewModel;

namespace QuizManagement.Application.Permissions.ViewModel
{
    public class PermissionViewModel
    {
        public int Id { get; set; }

        public Guid RoleId { get; set; }

        public string FunctionId { get; set; }

        public bool CanCreate { set; get; }

        public bool CanRead { set; get; }

        public bool CanUpdate { set; get; }

        public bool CanDelete { set; get; }

        public AppRoleViewModel AppRole { get; set; }

        public FunctionViewModel Function { get; set; }
    }
}
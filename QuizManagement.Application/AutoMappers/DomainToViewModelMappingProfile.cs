using AutoMapper;
using QuizManagement.Application.Chapters.ViewModel;
using QuizManagement.Application.Functions.ViewModel;
using QuizManagement.Application.Permissions.ViewModel;
using QuizManagement.Application.Roles.ViewModel;
using QuizManagement.Application.Users.ViewModel;
using QuizManagement.Data.Entities.Quiz;
using QuizManagement.Data.Entities.System;

namespace QuizManagement.Application.AutoMappers
{
    public class DomainToViewModelMappingProfile : Profile
    {
        public DomainToViewModelMappingProfile()
        {
            CreateMap<Chapter, ChapterViewModel>();

            CreateMap<Function, FunctionViewModel>();
            CreateMap<AppRole, AppRoleViewModel>();
            CreateMap<AppUser, AppUserViewModel>();
            CreateMap<Permission, PermissionViewModel>();
        }
    }
}
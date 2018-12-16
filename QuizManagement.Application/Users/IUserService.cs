using System.Collections.Generic;
using System.Threading.Tasks;
using QuizManagement.Application.Users.ViewModel;
using QuizManagement.Utilities.Paging;

namespace QuizManagement.Application.Users
{
    public interface IUserService
    {
        Task<bool> AddAsync(AppUserViewModel userVm);

        Task DeleteAsync(string id);

        Task<List<AppUserViewModel>> GetAllAsync();

        PagedResult<AppUserViewModel> GetAllPagingAsync(string keyword, int page, int pageSize);

        Task<AppUserViewModel> GetByIdAsync(string id);

        Task<bool> UpdateAsync(AppUserViewModel userVm);

        Task<bool> UpdateAccount(AppUserViewModel userVm);

        Task<List<AppUserViewModel>> AnnouncementUsers(string functionId);

        Task<bool> ChangePassword(string userId, string oldPassword, string password);

        Task<bool> ResetPassword(string userId, string password);

        bool CheckPhoneNumber(string phoneNumber);

        Task<bool> CheckUpdatePhoneNumber(string phoneNumber, string userId);
    }
}
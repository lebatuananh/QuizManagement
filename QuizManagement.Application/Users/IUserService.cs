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
    }
}
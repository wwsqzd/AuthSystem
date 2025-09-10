using AuthSystem.Core.Common;
using AuthSystem.Core.DTOs;
using AuthSystem.Core.Entities;

namespace AuthSystem.Core.Interfaces
{
    // ну просто интерфейс чтоб описывать работу че кто делает и че возвращает
    public interface IUserService
    {
        Task<List<UserDTO>> GetUsersAsync();
        Task<UserEntity?> GetUserByIdAsync(Guid id);
        Task<UserEntity?> GetUserByEmailAsync(string email);
        Task<Result<RegisterDTO>> AddUserAsync(RegisterDTO user);
        Task<bool> DeleteUserAsync(Guid id);
    }
}

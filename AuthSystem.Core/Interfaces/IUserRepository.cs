using AuthSystem.Core.DTOs;
using AuthSystem.Core.Entities;

namespace AuthSystem.Core.Interfaces
{
    public interface IUserRepository
    {
        Task AddUserAsyncRepo(RegisterDTO user);
        Task<bool> DeleteUserAsyncRepo(Guid id);
        Task<UserEntity?> GetUserByEmailAsyncRepo(string email);
        Task<UserEntity?> GetUserByIdAsyncRepo(Guid id);
        Task<List<UserDTO>> GetUsersAsyncRepo();
    }
}
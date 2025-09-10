using AuthSystem.Core.Common;
using AuthSystem.Core.DTOs;
using AuthSystem.Core.Entities;
using AuthSystem.Core.Interfaces;

namespace AuthSystem.Core.Services
{
    public class UserService : IUserService
    {
        private readonly IPasswordService _passwordService;
        private readonly IUserRepository _userRepository;

        public UserService(IPasswordService passwordService, IUserRepository userRepository)
        {
            _passwordService = passwordService;
            _userRepository = userRepository;
        }

        // добавляем пользователя
        public async Task<Result<RegisterDTO>> AddUserAsync(RegisterDTO user)
        {
            user.Password = _passwordService.HashPassword(user.Password);
            if (await _userRepository.GetUserByEmailAsyncRepo(user.Email) != null)
            {
                return Result<RegisterDTO>.Fail("This user already exists");
            }
            await _userRepository.AddUserAsyncRepo(user);
            return Result<RegisterDTO>.Ok(user);
        }

        // всех users мапим
        public async Task<List<UserDTO>> GetUsersAsync()
        {
            return await _userRepository.GetUsersAsyncRepo();
        }

        // Ищем по id
        public async Task<UserEntity?> GetUserByIdAsync(Guid id)
        {
            return await _userRepository.GetUserByIdAsyncRepo(id);
        }

        // ищем по имейлу
        public async Task<UserEntity?> GetUserByEmailAsync(string email)
        {
            return await _userRepository.GetUserByEmailAsyncRepo(email);
        }

        // удаляем пользователя
        public async Task<bool> DeleteUserAsync(Guid id)
        {
            return await _userRepository.DeleteUserAsyncRepo(id);
        }
    }
}

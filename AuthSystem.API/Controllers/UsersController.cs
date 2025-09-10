using AuthSystem.Core.DTOs;
using AuthSystem.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthSystem.API.Controllers
{
    // это контроллер собственно уже для управления users
    [Route("/")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        // принимает сервис
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        // тут чето типа загрузки текущего профиля человека
        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetMe()
        {
            // берем Id из токена(из клейма)
            var userIdClaim = User.FindFirst("UserId")?.Value;
            // нету ? значит не авторизован
            if (userIdClaim is null)
                return Unauthorized();
            // пробуем запарсить
            if (!Guid.TryParse(userIdClaim, out var userId))
                return BadRequest("Invalid user id in token");
            // ищем это айди
            var user = await _userService.GetUserByIdAsync(userId);
            if (user is null)
                return NotFound();
            // возвращаем если нашли
            return Ok(new
            {
                user!.Id,
                user.Name,
                user.Email,
                user.IsAdmin,
                user.CreatedAt
            });
        }

        // разрешенно только если пользователь админ
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<List<UserDTO>>> GetUsers()
        {
            var users = await _userService.GetUsersAsync();
            return Ok(users);
        }

        // разрешенно только если пользователь админ
        [Authorize(Roles = "Admin")]
        [HttpGet("profile/{id}")]
        public async Task<ActionResult<UserDTO>> GetUserById(Guid id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            return Ok(user);
        }



        // разрешенно только если пользователь админ
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var result = await _userService.DeleteUserAsync(id);
            return result ? NoContent() : NotFound();
        }
    }
}

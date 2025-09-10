using AuthSystem.Core.DTOs;
using AuthSystem.Core.Interfaces;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthSystem.API.Controllers
{
    // Контролллер аунтефикации и авторизации
    [Route("auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        // берет сервисы всякие
        private readonly IUserService _userService;
        private readonly IConfiguration _config;
        private readonly IPasswordService _passwordService;
        private readonly AbstractValidator<RegisterDTO> _register_validator;
        private readonly AbstractValidator<LoginDTO> _login_validator;


        // Контструктор
        public AuthController(IUserService userService, IConfiguration config, IPasswordService passwordService, AbstractValidator<RegisterDTO> register_validator, AbstractValidator<LoginDTO> login_validator)
        {
            _userService = userService;
            _config = config;
            _passwordService = passwordService;
            _register_validator = register_validator;
            _login_validator = login_validator;
        }

        // для ветки "auth/login"
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO dto)
        {
            ValidationResult resValidator = _login_validator.Validate(dto);
            if (!resValidator.IsValid)
            {
                return BadRequest("Incorrectly entered data");
            }
            // ищем юсера через сервис по email 
            var user = await _userService.GetUserByEmailAsync(dto.Email);

            // если пароль нулевой или не равен то ошибку кидаем
            if (user == null)
                return Unauthorized("Invalid email or password");
            bool isPasswordValid = _passwordService.VerifyPassword(user.Password, dto.Password);

            if (!isPasswordValid)
                return Unauthorized("Invalid password");

            // это клеймы внутри нашего jwt токена. тут и имя и все роли и инфа
            var claims = new[]
            {
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Role, user.IsAdmin ? "Admin" : "User"),
            new Claim("UserId", user.Id.ToString())
            };
            // так тут берем наш секретный пароль для создания jwt токена
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // генерация токена
            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );
            // возвращаем
            return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
        }

        // для ветки "auth/register"
        [HttpPost("register")]
        public async Task<IActionResult> AddUser(RegisterDTO user)
        {
            ValidationResult resValidator = _register_validator.Validate(user);
            if (!resValidator.IsValid)
            {
                return BadRequest("Incorrectly entered data");
            }
            var result = await _userService.AddUserAsync(user);
            if (!result.Success)
            {
                return Conflict(result.Error);
            }
            return Ok();
            
        }

        
    }
}

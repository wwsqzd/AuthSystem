
namespace AuthSystem.Core.DTOs
{
    // дтошка в плане какие данные должны прийти при логине
    public class LoginDTO
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}


namespace AuthSystem.Core.Interfaces
{
    // Interface for Password Service
    public interface IPasswordService
    {
        string HashPassword(string password);
        bool VerifyPassword(string hashedPassword, string providedPassword);
    }
}

using Microsoft.EntityFrameworkCore;
using AuthSystem.Core.Entities;



namespace AuthSystem.DataAccess.Context
{
    // ну тут создание обычного дб контекста для работы с Entity Framework
    public class ApplicationDbContext : DbContext
    {
        // тут создаем таблицу
        public DbSet<UserEntity> Users { get; set; }
        // тут гарантируем что будет создана в конструкторе
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
    }
}

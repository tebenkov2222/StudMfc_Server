using Microsoft.EntityFrameworkCore;

namespace ISTU_MFC.Models
{
    public class UserContext: DbContext
    {
        public DbSet<User> Users { get; set; }
        public UserContext(DbContextOptions<UserContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
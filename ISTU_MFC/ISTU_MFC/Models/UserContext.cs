using Microsoft.EntityFrameworkCore;

namespace ISTU_MFC.Models
{
    public class UserContext: DbContext
    {
        public DbSet<user> users { get; set; }
        public UserContext(DbContextOptions<UserContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
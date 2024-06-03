using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using truthfulls.com.Models;


namespace truthfulls.com.Data
{
    public class UserContext : IdentityDbContext<AppUser, IdentityRole,string>
    {
        public DbSet<UserComments> Comments { get; set; }
        public override DbSet<AppUser> Users { get; set; }
        public UserContext(DbContextOptions<UserContext> options) : base(options)
        {

        }
      
    }
}

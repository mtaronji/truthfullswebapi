
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;




namespace truthfulls.com.Data
{
    public class UserContext : IdentityDbContext<IdentityUser,IdentityRole,string>
    {

        public UserContext(DbContextOptions<UserContext> options) : base(options)
        {

        }
      
    }
}

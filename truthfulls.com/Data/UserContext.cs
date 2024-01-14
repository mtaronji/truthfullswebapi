using Azure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using truthfulls.com.Models;
using truthfulls.com.StockModels;


namespace truthfulls.com.Data
{
    public class UserContext : IdentityDbContext<IdentityUser,IdentityRole,string>
    {

        public UserContext(DbContextOptions<UserContext> options) : base(options)
        {

        }
      
    }
}

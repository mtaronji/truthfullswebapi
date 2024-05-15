using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace truthfulls.com.Models
{
    [Table("Feedback")]
    public class UserComments
    {
  
       [ForeignKey("AppUser")]
       public required string Id { get; set; }
       public required AppUser identityuser { get; set; }
       public required DateTime PostTime { get; set;}
       public required string message { get; set; }
    }

    public class AppUser : IdentityUser
    {
        public ICollection<UserComments>? Comments { get; set; }
    }

}

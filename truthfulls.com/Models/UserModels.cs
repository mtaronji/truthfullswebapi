namespace truthfulls.com.Models;

public class LoginVM
{
    public required string username { get; set; }
    public required string email { get; set; }
    public required string[] roles { get; set; }
}

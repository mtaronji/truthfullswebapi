using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using truthfulls.com.Models;
using System.Web;
using truthfulls.com.Data;



namespace truthfulls.com.Controllers
{
    [ApiController]
    public class AccountController : ControllerBase
    {
        private SignInManager<AppUser> _signinmanager;
        private UserManager<AppUser> _userManager;
        private RoleManager<IdentityRole> _roleManager;
        private UserContext _userContext;
        private string externalcallbackurl;


        public AccountController(SignInManager<AppUser> signinmanager, UserManager<AppUser> userManager, RoleManager<IdentityRole> rolemanager, UserContext usercontext)
        {
            this._userContext = usercontext;
            this._signinmanager = signinmanager;
             this._userManager = userManager;
            this._roleManager = rolemanager;

            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development") { this.externalcallbackurl = "https://localhost:50814/account/externallogincallback"; }
            else { this.externalcallbackurl = "https://datanets.azurewebsites.net/account/externallogincallback"; }
        }

        //if we are loggin in from our angular front end in development mode, we will have a different url.
        //therefore, we w
        [HttpGet]
        [Route("[controller]/externallogin/{provider}/{redirecturl?}")]

        public IActionResult ExternalLogin(string provider, string? redirecturl = null)
        {

            if (redirecturl != null) { redirecturl = HttpUtility.UrlEncode(redirecturl); }

            var properties = this._signinmanager.ConfigureExternalAuthenticationProperties(provider, "");
            properties.RedirectUri = $"{this.externalcallbackurl}/{redirecturl}";

            return Challenge(properties, provider);

        }

        [HttpGet]
        [Route("[controller]/externallogincallback/{redirecturl?}")]
        public async Task<IActionResult> ExternalLoginCallback(string? redirecturl = null)
        {
            string returnURL;
            if (redirecturl == null)
            {
                returnURL = $"{this.Request.Scheme}://{this.Request.Host.Value}";
            }
            else
            {
                returnURL = HttpUtility.UrlDecode(redirecturl);
            }


            var info = await this._signinmanager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                var errorModel = new { error = "External login failed. Please try another method" };
                return BadRequest(errorModel);
            }
            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            if (email == null) { return RedirectToPage("Error", new { error = "Third Party signin Provider ins't sharing your email address. Check Settings in the provider" }); }


            var user = await this._userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new();
                user.Email = email;
                user.UserName = email;
                var createuserresult = await this._userManager.CreateAsync(user);
                if (createuserresult != IdentityResult.Success)
                {
                    var errorModel = new { error = "Error creating a new User" };
                    return BadRequest(errorModel);
                }
                var role = new IdentityRole("user");
                await _roleManager.CreateAsync(role);
                await _userManager.AddToRoleAsync(user, "user");

                var claimaddresult = await this._userManager.AddClaimsAsync(user, info.Principal.Claims);
                if (claimaddresult != IdentityResult.Success) { return BadRequest(new { error = "Error adding claims for user" }); }

                var addloginresult = await this._userManager.AddLoginAsync(user, info);
                if (addloginresult != IdentityResult.Success) { return BadRequest(new { error = "Error adding login for user" }); }

            }

            //try to sign in with external login info. If it fails, try adding login and signing in again
            //if that fails return bad request
            var result = await this._signinmanager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, true);
            if (result == Microsoft.AspNetCore.Identity.SignInResult.Success) { return Redirect(returnURL); }
            else
            {
                var addloginresult = await this._userManager.AddLoginAsync(user, info);
                if (addloginresult != IdentityResult.Success) { return BadRequest(new { error = "Error adding login for user" }); }
                result = await this._signinmanager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);
                if (result == Microsoft.AspNetCore.Identity.SignInResult.Success) { return Redirect(returnURL); }
                else { return BadRequest(new { error = "Error signing In" }); }
            }
        }

        [HttpGet]
        [Route("[controller]/authenicationdetails")]
        public async Task<IActionResult> AuthenicationDetails()
        {

            if (User.Identity == null) { return Unauthorized(); }

            if (User.Identity.IsAuthenticated)
            {
                var user = await this._userManager.GetUserAsync(User);
                if (user == null || user.Email == null || user.UserName == null) { return NotFound(); }
                var rolenames = this._roleManager.Roles.ToList();
                if (rolenames.Count == 0) { return NotFound(); }
                string[] roles = { };
                foreach (var role in rolenames)
                {
                    if (role.Name == null) { return NotFound(); }
                    if (User.IsInRole(role.Name)) { roles.Append(role.Name); }
                }
                return Ok(new LoginVM() { email = user.Email, username = user.UserName, roles = roles });
            }
            else { return Ok(null); }

        }

        [HttpGet]
        [Route("[controller]/isauthenticated")]
        public IActionResult IsAuthenticated()
        {
            if (User.Identity == null)
            {
                return Unauthorized(false);
            }
            else
            {
                if(User.Identity.IsAuthenticated)
                {
                    return Ok(true);
                }
                else
                {
                    return Unauthorized(false);
                }
            }

        }
        [HttpGet]
        [Route("[controller]/externalsignout")]
        public async Task<IActionResult> ExternalSignout()
        {
            await this._signinmanager.SignOutAsync();
            return Ok();
        }

       
        [HttpPost]
        [Route("[controller]/feedback")]
        public async Task<IActionResult> Feedback([FromBody] string comment)
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null) { return NotFound(); }
                var user = this._userManager.Users.FirstOrDefault(x => x.Id == userId);
                if(user == null) { return NotFound(); }

                UserComments usercomment = new UserComments()
                {
                    message = comment,
                    Id = userId,
                    identityuser = user,
                    PostTime = DateTime.UtcNow
                };
                this._userContext.Add(usercomment);
                await this._userContext.SaveChangesAsync();
                return Ok(true);
            }
            else
            {
                return BadRequest();
            }
        }
        [HttpGet]
        [Route("isuser")]
        public IActionResult IsUser()
        {
            var isUser = User.IsInRole("User");
            return Ok(new { isuser = isUser });
        }

        [HttpGet]
        [Route("issubscriber")]
        public IActionResult IsSubscriber()
        {
            var isSubscriber = User.IsInRole("Subscriber");
            return Ok(new { issubscriber = isSubscriber });
        }

    }
}

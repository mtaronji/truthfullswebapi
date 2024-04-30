using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using truthfulls.com.Models;
using System.Web;
using System.Diagnostics.Eventing.Reader;


namespace truthfulls.com.Controllers
{
    [ApiController]
    public class AccountController : ControllerBase
    {
        private SignInManager<IdentityUser> _signinmanager;
        private UserManager<IdentityUser> _userManager;
        private RoleManager<IdentityRole> _roleManager;
        private string externalcallbackurl;


        public AccountController(SignInManager<IdentityUser> signinmanager, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> rolemanager)
        {
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
            if (email == null) { return RedirectToPage("Error", new { error = "Could not find an email address" }); }


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
                var claimaddresult = await this._userManager.AddClaimsAsync(user, info.Principal.Claims);
                if (claimaddresult != IdentityResult.Success) { return BadRequest(new { error = "Error adding claims for user" }); }

                var addloginresult = await this._userManager.AddLoginAsync(user, info);
                if (addloginresult != IdentityResult.Success) { return BadRequest(new { error = "Error adding login for user" }); }

            }

            //try to sign in with external login info. If it fails, try adding login and signing in again
            //if that fails return bad request
            var result = await this._signinmanager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);
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
        [Route("[controller]/isauthenicated")]
        public async Task<IActionResult> IsAuthenicated()
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
        [Route("[controller]/externalsignout")]
        public async Task<IActionResult> ExternalSignout()
        {
            await this._signinmanager.SignOutAsync();
            return Ok();
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

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using System.Text.Json.Serialization;
using truthfulls.com.Data;
using truthfulls.com.Models;
using truthfulls.com.Services;



var builder = WebApplication.CreateBuilder(args);

//check connection string for config error

//temp regex check

var angularDevelopLocalIP = "_angularDevelopLocalIP";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: angularDevelopLocalIP,
                      policy =>
                      {
                          policy.WithOrigins("http://localhost:4200").AllowAnyHeader().AllowCredentials(); //for development of angular front end
                          policy.WithOrigins("https://localhost:50814");

                      });
});

builder.Services.AddControllers().AddJsonOptions(
    //don't send null values, ignore them
    options => { options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull; }
);
builder.Services.AddRazorPages();
builder.Services.AddMemoryCache();
builder.Services.Configure<KestrelServerOptions>(options =>
{
    options.Limits.MaxRequestBodySize = 300000000;
});
//add cors for development with angular 


builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<UserContext>(options =>
{
    if (builder.Environment.IsEnvironment("Development"))
    {
        options.UseSqlite(builder.Configuration.GetConnectionString("CUSTOMCONNSTR_identity"));
    }
    else
    {
        options.UseSqlite(Environment.GetEnvironmentVariable("CUSTOMCONNSTR_identity"));
    }
    
});


builder.Services.AddDefaultIdentity<AppUser>(options =>
{
   
})
.AddRoles<IdentityRole>().AddSignInManager()
.AddEntityFrameworkStores<UserContext>();

builder.Services.AddScoped<PunkInterpreter>();
builder.Services.AddScoped<PunkPostParser>();



builder.Services.AddAuthentication()

    .AddTwitter(twitteroptions =>
    {
        twitteroptions.SignInScheme = IdentityConstants.ExternalScheme;
        twitteroptions.ConsumerKey = Environment.GetEnvironmentVariable("Twitter:CAPI");
        twitteroptions.ConsumerSecret = Environment.GetEnvironmentVariable("Twitter:Secret");
        twitteroptions.RetrieveUserDetails = true;
    })

    .AddFacebook(facebookoptions =>
    {
        string? fbid = Environment.GetEnvironmentVariable("FB:ID"); string? fbs = Environment.GetEnvironmentVariable("FB:S");
        if (fbid == null || fbs == null) { throw new ArgumentNullException(); }

        facebookoptions.AppId = fbid;
        facebookoptions.AppSecret = fbs;
        facebookoptions.SignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddGoogle(googleoptions =>
    {
        string? gid = Environment.GetEnvironmentVariable("G:ID"); string? gs = Environment.GetEnvironmentVariable("G:S");
        if (gid == null || gs == null) { throw new ArgumentNullException(); }
        googleoptions.SignInScheme = IdentityConstants.ExternalScheme;
        googleoptions.ClientId = gid;
        googleoptions.ClientSecret = gs;

    });

string? env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

//we are putting it so that the cookies can come from places other than the site
//this is for configuring
if (env == "Development")
{
    builder.Services.Configure<CookieAuthenticationOptions>(IdentityConstants.ApplicationScheme,
    x => x.Cookie.SameSite = SameSiteMode.None);
}


var app = builder.Build();

app.UseRouting();

if (app.Environment.IsDevelopment())
{
    app.UseCors(angularDevelopLocalIP);

}

//must configure a default file provider so that our angular app gets served from the root
DefaultFilesOptions options = new DefaultFilesOptions();
options.FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "truthfulls-ui"));
app.UseDefaultFiles(options);

//Tell the middle ware we will use static files from the following location as the default files
app.UseStaticFiles(new StaticFileOptions
{
    ServeUnknownFileTypes = true,
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "truthfulls-ui")),
    RequestPath = ""
});



app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapRazorPages();

app.Run();

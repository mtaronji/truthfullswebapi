using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using System;
using System.Text.Json.Serialization;
using truthfulls.com.Models.PlotlyModels;
using truthfulls.com.Services;



var builder = WebApplication.CreateBuilder(args);

//check connection string for config error

builder.Services.AddSingleton<StockDataRetrievalService>();
builder.Services.AddSingleton<OptionRetrievalService>();

builder.Services.AddControllers().AddJsonOptions(
    //don't send null values, ignore them
    options => { options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull; }
);

builder.Services.AddMemoryCache();

//add cors for development with angular 
var angularDevelopLocalIP = "_angularDevelopLocalIP";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: angularDevelopLocalIP,
                      policy =>
                      {
                          policy.WithOrigins("http://localhost:4200"); //for development of angular front end
                      });
});

//IHostEnvironment env = builder.Environment;
//builder.Configuration.AddJsonFile($"appsettings.{env.EnvironmentName}.json");

builder.Configuration.AddEnvironmentVariables();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
   
    app.UseExceptionHandler("/Error");

    // The default HSTS value is 30 days. You may want to change this for production scenarios,see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    app.UseHttpsRedirection();
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

if (app.Environment.IsDevelopment())
{
    app.UseCors(angularDevelopLocalIP);
}
app.UseRouting();
app.MapControllers();

app.UseAuthentication();
app.UseAuthorization();


app.Run();

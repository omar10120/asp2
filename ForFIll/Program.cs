using ForFIll.Components;
using MudBlazor.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using ForFIll.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Net.Http;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ForFIll.Models;
using System.Net.NetworkInformation;
using Microsoft.EntityFrameworkCore.Query.Internal;
using ForFIll.Services.Interfaces;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddControllersWithViews();

/*************************************************************User Auther*/

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie();
//builder.Services.AddAuthorizationCore();
builder.Services.AddAuthorizationCore(options =>
{
    options.AddPolicy("admin", policy =>
        policy.RequireAssertion(context =>
            context.User.Identity != null &&
            context.User.Identity.IsAuthenticated &&
            context.User.Identity.Name == "admin") );


    // Admin policy requiring "Role" claim with value "Admin" and "Permission" claim with value "ManageUsers"
    //options.AddPolicy("AdminPolicy", policy =>
    //    policy.RequireClaim(ClaimTypes.Role, "Admin")
    //          .RequireClaim("Permission", "ManageUsers"));

    // Editor policy requiring "Role" claim with value "Editor"
    //options.AddPolicy("EditorPolicy", policy =>
    //    policy.RequireClaim(ClaimTypes.Role, "Editor"));
});
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("CanEditProduct", policy =>
        policy.RequireClaim("EditProduct", "true"));
});

builder.Services.AddAuthorization();
// Register IHttpContextAccessor
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<CreateDatabase>();
builder.Services.AddSingleton<AppState>();




builder.Services.AddAntiforgery(options => options.HeaderName = "X-CSRF-TOKEN");

/*************************************************************User Auther*/



//builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer($"Server={Environment.MachineName}\\{Environment.UserName};Database=TS_TestUser;Trusted_Connection=True;TrustServerCertificate=True;"));
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer($"Server={Environment.MachineName};Database=TS_TestUser;Trusted_Connection=True;TrustServerCertificate=True;"));


builder.Services.AddMudServices(); // Add MudBlazor services
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:5001") });
builder.Services.AddScoped<ProductService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
app.UseExceptionHandler("/Error", createScopeForErrors: true);

app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapControllers();


app.MapBlazorHub("/App"); //problem here
app.UseAntiforgery();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();


app.Run();








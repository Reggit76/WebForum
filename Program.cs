using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebForum.Data;
using WebForum.Models;
using WebForum.Services;

var builder = WebApplication.CreateBuilder(args);

// Configure services
ConfigureServices(builder.Services, builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline
Configure(app, app.Environment);

app.Run();

void ConfigureServices(IServiceCollection services, IConfiguration configuration)
{
    services.AddControllersWithViews();
    services.AddDbContext<ApplicationDbContext>(options =>
        options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

    services.AddScoped<IUserService, UserService>();
    services.AddScoped<IForumService, ForumService>();
    services.AddScoped<ICategoryService, CategoryService>();

    services.AddIdentity<User, IdentityRole<int>>(options =>
    {
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = true;
        options.Password.RequiredLength = 6;
        options.Password.RequiredUniqueChars = 1;
    })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

    services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie(options =>
        {
            options.LoginPath = "/Account/Login";
            options.AccessDeniedPath = "/Account/AccessDenied";
        });

    services.AddHttpContextAccessor();
}

void Configure(WebApplication app, IWebHostEnvironment env)
{
    if (!env.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();

    app.UseRouting();

    app.UseAuthentication();
    app.UseBanCheck();
    app.UseAuthorization();

    app.Use(async (context, next) =>
    {
        if (!context.User.Identity.IsAuthenticated)
        {
            var token = context.Request.Form["__RequestVerificationToken"];
            if (string.IsNullOrEmpty(token))
            {
                context.Response.Redirect("/Account/Login");
                return;
            }
        }
        await next();
    });

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    EnsureDatabaseCreated(app.Services).Wait();
}

async Task EnsureDatabaseCreated(IServiceProvider services)
{
    using var scope = services.CreateScope();
    var scopedServices = scope.ServiceProvider;
    var context = scopedServices.GetRequiredService<ApplicationDbContext>();
    context.Database.Migrate();

    await CreateRolesAsync(scopedServices);
    await CreateAdminUserAsync(scopedServices);
}

async Task CreateRolesAsync(IServiceProvider serviceProvider)
{
    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();

    string[] roleNames = { "Administrator", "Moderator", "RegularUser" };

    foreach (var roleName in roleNames)
    {
        var roleExist = await roleManager.RoleExistsAsync(roleName);
        if (!roleExist)
        {
            await roleManager.CreateAsync(new IdentityRole<int>(roleName));
        }
    }
}

async Task CreateAdminUserAsync(IServiceProvider serviceProvider)
{
    var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();

    string adminUsername = "GOD";
    string adminEmail = "admin@adminmail.com";
    string adminPassword = "Admin123!";

    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser == null)
    {
        adminUser = new User
        {
            UserName = adminUsername,
            Email = adminEmail,
            Role = Role.Administrator
        };

        var result = await userManager.CreateAsync(adminUser, adminPassword);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, "Administrator");
        }
    }
}

// Extension method used to add the middleware to the HTTP request pipeline.
public static class BanCheckMiddlewareExtensions
{
    public static IApplicationBuilder UseBanCheck(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<BanCheckMiddleware>();
    }
}

public class BanCheckMiddleware
{
    private readonly RequestDelegate _next;

    public BanCheckMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.User.Identity.IsAuthenticated)
        {
            var userManager = context.RequestServices.GetRequiredService<UserManager<User>>();
            var user = await userManager.GetUserAsync(context.User);
            if (user != null && user.IsBanned)
            {
                context.Response.Redirect("/Account/Banned");
                return;
            }
        }

        await _next(context);
    }
}

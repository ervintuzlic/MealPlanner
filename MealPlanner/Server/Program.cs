using MealPlanner.Application.Data;
using MealPlanner.Application.DomainModel;
using MealPlanner.Infrastructure.Configuration.Authorization;
using MealPlanner.Infrastructure.Extensions.Authorization;
using MealPlanner.Server;
using MealPlanner.Shared.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace MealPlanner;

public class Program
{
    internal static string ASPNETCORE_ENVIRONMENT = "Development";

    public static void Main(string[] args)
    {
        IConfigurationRoot config = BuildConfigRoot(ASPNETCORE_ENVIRONMENT, args);

        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
        });

        builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        var ioc = new DependencyInjector();
        ioc.RegisterServices(builder.Services);

        builder.Services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(30);
            options.Cookie.HttpOnly = true;
        });

        IConfiguration jwtSettings = builder.Configuration.GetSection("JwtSettings");

        var roleManager = builder.Services.GetRequiredService<RoleManager<IdentityRole>>();

        builder.Services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false,
                    ValidAudience = jwtSettings["Audience"],
                    ValidateIssuer = false,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidateLifetime = false,
                    ValidateIssuerSigningKey = false,
                    IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(jwtSettings["Key"]!)),
                };
            });

        builder.Services.AddAuthorization(options =>
        {
            options.DefaultPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                .Build();

            options.RegisterModulePolicies<ModuleCode>(configuration =>
            {
                configuration.ClaimType = AuthorizationPolicyRegistration.AuthorizationClaimType;

                configuration.AdditionalPolicies = new List<MultiModulePolicy>();

                configuration.AuthenticationScheme = JwtBearerDefaults.AuthenticationScheme;
            });
        });

        builder.Services.AddControllersWithViews();
        builder.Services.AddRazorPages();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseWebAssemblyDebugging();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();

        app.UseBlazorFrameworkFiles();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseSession();
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapRazorPages();
        app.MapControllers();
        app.MapFallbackToFile("index.html");

        app.Run();
    }
    static IConfigurationRoot BuildConfigRoot(string env, string[] args)
    {
        bool allowChange = string.Compare("Development", env, true) == 0;
        var builder = new ConfigurationBuilder()
            .AddJsonFile("appSettings.json", optional: true, reloadOnChange: allowChange)
            .AddEnvironmentVariables()
            .AddCommandLine(args);
        return builder.Build();
    }
}



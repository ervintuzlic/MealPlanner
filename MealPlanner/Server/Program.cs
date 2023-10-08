using MealPlanner.Application.Data;
using MealPlanner.Application.DomainModel;
using MealPlanner.Server;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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

        var ioc = new DependencyInjector();
        ioc.RegisterServices(builder.Services);

        builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();
            

        builder.Services.AddControllersWithViews();
        builder.Services.AddRazorPages();

        builder.Services.AddAuthorization();
        builder.Services.AddAuthentication();

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



using MealPlanner.Application.Services.Authorization;

namespace MealPlanner.Server;

public class DependencyInjector
{
    public void RegisterServices(IServiceCollection services)
    {
        services.AddScoped<IAuthorizationService, AuthorizationService>();
    }
}

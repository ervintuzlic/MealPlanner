using MealPlanner.Client.DataServices.AuthorizationDataService;

namespace MealPlanner.Client;

public class DependencyInjector
{
    public void RegisterServices(IServiceCollection services)
    {
        services.AddScoped<IAuthorizationDataService, AuthorizationDataService>();
    }
}

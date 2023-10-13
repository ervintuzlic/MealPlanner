using MealPlanner.Shared.DTO.Authorization;

namespace MealPlanner.Client.DataServices.AuthorizationDataService;

public interface IAuthorizationDataService
{
    Task<string> Login(LoginRequest request);

    Task Register(RegisterRequest request);

    Task Logout();
}

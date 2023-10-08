using MealPlanner.Shared.DTO.Authorization;

namespace MealPlanner.Client.DataServices.AuthorizationDataService;

public interface IAuthorizationDataService
{
    Task Login(LoginRequest request);

    Task Register(RegisterRequest request);
}

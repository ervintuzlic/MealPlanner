using MealPlanner.Application.DomainModel;
using MealPlanner.Shared.DTO.Authorization;

namespace MealPlanner.Application.Services.Authorization;

public interface IAuthorizationService
{
    Task<ApplicationUser?> Login(LoginRequest request);

    Task<ApplicationUser?> Register(RegisterRequest request);
}

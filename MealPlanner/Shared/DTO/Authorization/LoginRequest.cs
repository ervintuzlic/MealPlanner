namespace MealPlanner.Shared.DTO.Authorization;

public class LoginRequest
{
    public string? Email { get; set; }

    public string? Password { get; set; }

    public bool RememberMe { get; set; }
}

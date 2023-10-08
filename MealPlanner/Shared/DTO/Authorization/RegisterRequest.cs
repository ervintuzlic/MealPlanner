namespace MealPlanner.Shared.DTO.Authorization;

public class RegisterRequest
{
    public string? Email { get; set; }

    public string? Username { get; set; }

    public DateTime? DateOfBirth { get; set; }

    public string? Password { get; set; }
}

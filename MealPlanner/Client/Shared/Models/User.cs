using System.Security.Claims;

namespace MealPlanner.Client.Shared.Models;

public class User
{
    public string DisplayName { get; set; } = null!;

    public string JwtToken { get; set; } = null!;

    public ClaimsPrincipal ClaimsPrincipal { get; set; }

    public User(string name, string token, ClaimsPrincipal principal)
    {
        DisplayName = name;
        JwtToken = token;
        ClaimsPrincipal = principal;
    }
}

namespace MealPlanner.Infrastructure.Configuration.Authorization;

public class AuthorizationConfiguration<T> where T : Enum
{
    public string? ClaimType { get; set; }

    public List<MultiModulePolicy>? AdditionalPolicies { get; set; }

    public string? AuthenticationScheme { get; set; }
}

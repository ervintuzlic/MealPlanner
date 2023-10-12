namespace MealPlanner.Shared.Authorization;

public class AuthorizationPolicyRegistration
{
    public const string AuthorizationClaimType = "roles";

    public string[] Admin => new[] { "Hello" };
}

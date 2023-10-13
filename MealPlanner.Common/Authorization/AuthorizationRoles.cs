namespace MealPlanner.Common.Authorization;

public class AuthorizationRoles
{
    /// <summary>
    /// List of main roles
    /// </summary>
    public const string Admin = "Admin";
    public const string Free = "Free";
    public const string Premium = "Premium";

    public static List<string> Roles => new List<string> { Admin, Free, Premium };

    /// <summary>
    /// List of individual policy contained by main role
    /// </summary>
    public const string Counter = "Counter";

    /// <summary>
    /// Retrieve all individual policies
    /// </summary>
    public static List<string> Policies => new List<string> { Counter };
}

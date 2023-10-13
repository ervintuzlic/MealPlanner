namespace MealPlanner.Common.Authorization;

public class AuthorizationPolicyRegistration
{
    /// <summary>
    /// Authorization Claim Type for Jwt Token
    /// </summary>
    public const string AuthorizationClaimType = "role";

    /// <summary>
    /// List for each main role that contains individual policies
    /// </summary>
    public static List<string> Admin = new List<string> { AuthorizationRoles.Counter };
    public static List<string> Free = new List<string> { };
    public static List<string> Premium = new List<string> { AuthorizationRoles.Counter };

    /// <summary>
    /// Method to get list of individual policies by providing name of role from Db
    /// </summary>
    /// <param name="role">Role name associated with user in Db</param>
    /// <returns></returns>
    public static List<string> GetForRole(string role)
    {
        switch (role)
        {
            case nameof(Admin):
                return Admin;
            case nameof(Free):
                return Free;
            case nameof(Premium):
                return Premium;
            default: break;
        }

        return new List<string>();
    }
}

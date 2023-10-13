using MealPlanner.Common.Authorization;
using MealPlanner.Infrastructure.Configuration.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace MealPlanner.Infrastructure.Extensions.Authorization;

public static class AuthorizationConfigurationExtensions
{
    public static void RegisterModulePolicies(this AuthorizationOptions options, Action<AuthorizationConfiguration> configuration)
    { 
        var config = new AuthorizationConfiguration();

        configuration(config);

        if (config.ClaimType!.IsNullOrWhiteSpace())
        {
            throw new ArgumentException("Claim Type is not specified");
        }

        var policies = AuthorizationRoles.Policies;

        foreach(var policy in policies)
        {
            options.AddPolicy(policy, RegisterPolicy(config.ClaimType!, policy, config.AuthenticationScheme!));
        }
    }

    private static AuthorizationPolicy RegisterPolicy(string claimType, string policy, string authenticationScheme)
    {
        var authorizationPolicyBuilder = new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .RequireAssertion(x =>
            {
                var role = x.User.Claims
                    .Where(x => x.Type == claimType)
                    .FirstOrDefault()?.Value;

                if (role.IsNullOrWhiteSpace())
                {
                    return false;
                }

                var listOfPoliciesForRole = AuthorizationPolicyRegistration.GetForRole(role!);

                var hasPolicy = listOfPoliciesForRole
                    .Any(x => x == policy);

                return hasPolicy;
            });

        if(!authenticationScheme.IsNullOrWhiteSpace())
        {
            authorizationPolicyBuilder.AddAuthenticationSchemes(authenticationScheme);
        }

        var buildPolicyBuilder = authorizationPolicyBuilder.Build();

        return buildPolicyBuilder;
    }
}

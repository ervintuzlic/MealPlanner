using MealPlanner.Infrastructure.Configuration.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace MealPlanner.Infrastructure.Extensions.Authorization;

public static class AuthorizationConfigurationExtensions
{
    public static void RegisterModulePolicies<T>(this AuthorizationOptions options, Action<AuthorizationConfiguration<T>> configuration) where T : Enum
    { 
        var config = new AuthorizationConfiguration<T>();

        configuration(config);

        if (config.ClaimType!.IsNullOrWhiteSpace())
        {
            throw new ArgumentException("Claim Type is not specified");
        }
        
        var values = Enum.GetValues(typeof(T));

        foreach(var value in values)
        {
            options.AddPolicy(value.ToString()!, RegisterPolicy(config.ClaimType!, value.ToString()!, config.AuthenticationScheme!));
        }

        if (config.AdditionalPolicies?.Any() == true)
        {
            foreach(var policy in config.AdditionalPolicies)
            {
                if (policy.PolicyName.IsNullOrWhiteSpace())
                {
                    throw new ArgumentException("Module policy is not configured properly");
                }

                if(policy.ModuleCodes?.Any() == false)
                {
                    throw new ArgumentException("There are no module codes for this policy");
                }

                options.AddPolicy(policy.PolicyName, RegisterMultiModulePolicy(config.ClaimType!, policy, config.AuthenticationScheme!));
            }
        }
    }

    private static AuthorizationPolicy RegisterMultiModulePolicy(string claimType, MultiModulePolicy policy, string authenticationScheme)
    {
        var authorizationPolicyBuiler = new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .RequireAssertion(x =>
            {
                var listOfModules = policy.ModuleCodes.Select(x => x.ToString()).ToList();

                var hasPolicy = x.User.Claims
                .Where(x => x.Type == claimType)
                .SelectMany(c => c.Value.Split(','))
                .Any(s => listOfModules.Contains(s));

                return hasPolicy;
            });

        if(!authenticationScheme.IsNullOrWhiteSpace())
        {
            authorizationPolicyBuiler.AddAuthenticationSchemes(authenticationScheme);
        }

        var builder = authorizationPolicyBuiler.Build();

        return builder;
    }

    private static AuthorizationPolicy RegisterPolicy(string claimType, string module, string authenticationScheme)
    {
        var authorizationPolicyBuilder = new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .RequireAssertion(x =>
            {
                var hasPolicy = x.User.Claims
                    .Where(x => x.Type == claimType)
                    .SelectMany(x => x.Value.Split(','))
                    .Any(p => p == module);

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

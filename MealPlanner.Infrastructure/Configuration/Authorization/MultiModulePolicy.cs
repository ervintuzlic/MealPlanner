namespace MealPlanner.Infrastructure.Configuration.Authorization;

public record MultiModulePolicy(string roleName, params Enum[] listOfPolicies);

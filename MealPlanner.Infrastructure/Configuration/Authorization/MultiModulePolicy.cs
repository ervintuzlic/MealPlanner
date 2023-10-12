namespace MealPlanner.Infrastructure.Configuration.Authorization;

public record MultiModulePolicy(string PolicyName, params Enum[] ModuleCodes);

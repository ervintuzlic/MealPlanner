using MealPlanner.Common.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MealPlanner.Application.Data;

public static class SeedData
{
    public static async Task Seed(ApplicationDbContext context)
    {
        await SeedRoles(context);

        await context.SaveChangesAsync();
    }
    public static async Task SeedRoles(ApplicationDbContext context)
    {
        var roles = AuthorizationRoles.Roles;

        var existingRoles = await context.Roles.ToListAsync();

        foreach(var role in roles)
        {
            if(!existingRoles.Any(x => x.Name == role))
            {
                var identityRole = new IdentityRole()
                {
                    Name = role,
                    NormalizedName = role,
                };
                
                context.Roles.Add(identityRole);
            }
        }
    }
}


using Microsoft.AspNetCore.Identity;

namespace MealPlanner.Application.DomainModel;

public class ApplicationUser : IdentityUser
{
    public DateTime? DateOfBirth { get; set; }

    public byte[] Salt { get; set; } = null!;
}

using MealPlanner.Client.DataServices.AuthorizationDataService;
using MealPlanner.Shared.DTO.Authorization;
using Microsoft.AspNetCore.Components;

namespace MealPlanner.Client.Pages.Authorization;

partial class Register
{
    [Inject]
    private IAuthorizationDataService AuthorizationDataService { get; set; } = default!;

    private RegisterRequest RegisterRequest { get; set; } = new RegisterRequest();

    private string? ConfirmedPassword { get; set; }

    bool PasswordVisible { get; set; } = true;

    bool ConfirmedPasswordVisible { get; set; } = true;

    void TogglePassword() => PasswordVisible = !PasswordVisible;
    void ToogleConfirmedPassword() => ConfirmedPasswordVisible = !ConfirmedPasswordVisible;

    private async void OnSubmit()
    {
        try
        {
            await AuthorizationDataService.Register(RegisterRequest);
        }
        catch (Exception ex)
        {
            Console.WriteLine("error while registration");
        }
    }
    private void OnInvalidSubmit()
    {

    }
}

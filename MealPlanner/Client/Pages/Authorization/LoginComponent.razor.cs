using MealPlanner.Client.DataServices.AuthorizationDataService;
using MealPlanner.Client.Providers;
using MealPlanner.Shared.DTO.Authorization;
using Microsoft.AspNetCore.Components;

namespace MealPlanner.Client.Pages.Authorization;

partial class LoginComponent
{
    [Inject]
    private IAuthorizationDataService AuthorizationDataService { get; set; } = default!;

    [Inject]
    private JwtAuthenticationStateProvider JwtAuthenticationStateProvider { get; set; } = default!;

    private LoginRequest LoginRequest { get; set; } = new LoginRequest();

    bool PasswordVisible { get; set; } = true;

    void TogglePassword() => PasswordVisible = !PasswordVisible;

    private async void OnSubmit()
    {
        try
        {
            var token = await AuthorizationDataService.Login(LoginRequest);

            JwtAuthenticationStateProvider.Login(token);
        }
        catch (Exception ex)
        {
            Console.WriteLine("error while login");
        }
    }
    private void OnInvalidSubmit()
    {

    }
}

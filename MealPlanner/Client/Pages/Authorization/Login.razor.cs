﻿using MealPlanner.Client.DataServices.AuthorizationDataService;
using MealPlanner.Shared.DTO.Authorization;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace MealPlanner.Client.Pages.Authorization;

partial class Login
{
    [Inject]
    private IAuthorizationDataService AuthorizationDataService { get; set; } = default!;

    private LoginRequest LoginRequest { get; set; } = new LoginRequest();

    bool PasswordVisible { get; set; } = true;

    void TogglePassword() => PasswordVisible = !PasswordVisible;

    private async void OnSubmit()
    {
        try
        {
            await AuthorizationDataService.Login(LoginRequest);
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

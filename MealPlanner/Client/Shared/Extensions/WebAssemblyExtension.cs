using MealPlanner.Client.Providers;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System.Net.Http.Json;

namespace MealPlanner.Client.Shared.Extensions;

public static class WebAssemblyExtension
{
    public static async Task RefreshToken(this WebAssemblyHost app)
    {
        using var boostrapScope = app.Services.CreateScope();
        using var api = boostrapScope.ServiceProvider.GetRequiredService<HttpClient>();

        HttpContent httpContent = new StringContent(string.Empty);
        var refreshTokenResponse = await api.PostAsJsonAsync($"api/Authorization/refresh", httpContent);

        if (refreshTokenResponse.IsSuccessStatusCode)
        {
            var content = await refreshTokenResponse.Content.ReadAsStringAsync();

            var loginStateService = boostrapScope.ServiceProvider.GetRequiredService<JwtAuthenticationStateProvider>();
            loginStateService.Login(content);
        }
    }
}

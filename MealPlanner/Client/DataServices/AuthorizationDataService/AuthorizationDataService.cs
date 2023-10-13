using MealPlanner.Shared.DTO.Authorization;
using System.Net.Http.Json;

namespace MealPlanner.Client.DataServices.AuthorizationDataService;

public class AuthorizationDataService : IAuthorizationDataService
{
    private readonly HttpClient _httpClient;

    public AuthorizationDataService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> Login(LoginRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync($"api/Authorization/login", request);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Something went wrong");
        }

        var result = await response.Content.ReadAsStringAsync();

        return result ?? "";
    }

    public async Task Register(RegisterRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("api/Authorization/register", request);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Not registered");
        }
    }

    public async Task Logout()
    {
        HttpContent httpContent = new StringContent(string.Empty);

        var response = await _httpClient.PostAsJsonAsync("api/Authorization/logout", httpContent);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Error while logging out");
        }
    }
}

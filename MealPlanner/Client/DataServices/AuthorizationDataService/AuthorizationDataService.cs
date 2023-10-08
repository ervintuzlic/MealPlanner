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

    public async Task Login(LoginRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync($"api/Authorization/login", request);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Something went wrong");
        }
    }

    public async Task Register(RegisterRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("api/Authorization/register", request);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Not registered");
        }

        //Testing commit
    }
}

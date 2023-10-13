using MealPlanner.Client;
using MealPlanner.Client.Handlers;
using MealPlanner.Client.Providers;
using MealPlanner.Client.Shared.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MealPlanner.Infrastructure.Extensions.Authorization;
using MealPlanner.Common.Authorization;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddAuthorizationCore(options =>
{
    options.DefaultPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .AddAuthenticationSchemes("Bearer")
        .Build();

    options.RegisterModulePolicies(configuration =>
    {
        configuration.ClaimType = AuthorizationPolicyRegistration.AuthorizationClaimType;

        configuration.AuthenticationScheme = "Bearer";
    });
});

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddSingleton<JwtAuthenticationStateProvider>();
builder.Services.AddSingleton<AuthenticationStateProvider>(provider => provider.GetRequiredService<JwtAuthenticationStateProvider>());

var appUri = new Uri(builder.HostEnvironment.BaseAddress);

builder.Services.AddScoped(provider => new JwtTokenMessageHandler(appUri, provider.GetRequiredService<JwtAuthenticationStateProvider>()));
builder.Services.AddHttpClient("MealPlanner.ServerAPI", client => client.BaseAddress = appUri)
    .AddHttpMessageHandler<JwtTokenMessageHandler>();
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("MealPlanner.ServerAPI"));

var ioc = new DependencyInjector();
ioc.RegisterServices(builder.Services);

var app = builder.Build();

await app.RefreshToken();

await app.RunAsync();
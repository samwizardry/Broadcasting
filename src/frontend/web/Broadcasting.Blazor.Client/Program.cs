using Broadcasting.Blazor.Client.HttpClients;
using Broadcasting.Blazor.Client.Services;

using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthenticationStateDeserialization();

builder.Services.AddClientServices(builder.HostEnvironment.BaseAddress);

await builder.Build().RunAsync();

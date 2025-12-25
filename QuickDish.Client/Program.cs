using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using QuickDish.Client;
using QuickDish.Client.Auth;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddAuthServices();
builder.Services.RegisterServices();

await builder.Build().RunAsync();
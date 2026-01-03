using ActualLab.Fusion.Server;
using ActualLab.Rpc.Server;
using SmartQuiz;
using SmartQuiz.Client;
using SmartQuiz.Components;
using _Imports = SmartQuiz.Client._Imports;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.RegisterFusionServices(builder.Configuration);

builder.Services.RegisterSharedServices();
builder.Services.AddAuthorization();

builder.Services.AddCors(options => options.AddDefaultPolicy(policy =>
{
    policy.WithOrigins("http://localhost:5026", "https://localhost:7157")
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
}));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseCors();
app.MapStaticAssets();

app.UseWebSockets(new WebSocketOptions()
{
    KeepAliveInterval = TimeSpan.FromSeconds(30),
});

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseFusionSession();
app.UseAntiforgery();


app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(_Imports).Assembly);

app.MapRpcWebSocketServer();
app.MapFusionAuthEndpoints();
app.MapFusionRenderModeEndpoints();

app.Run();
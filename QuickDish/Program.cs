using Microsoft.EntityFrameworkCore;
using QuickDish.Client;
using QuickDish.Auth;
using QuickDish.Data;
using QuickDish.Components;
using _Imports = QuickDish.Client._Imports;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.RegisterServices();

builder.Services.AddControllers();
builder.Services.AddCors(options => options.AddDefaultPolicy(policy =>
{
    policy.AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod();
}));

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options => { options.UseNpgsql(connectionString); });

builder.Services.AddAuthServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}

//app.UseHttpsRedirection();
app.UseCors();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(_Imports).Assembly);


app.MapControllers();

app.MapLoginAndLogout();
//app.MapRegisterUser();
//app.MapGetUserProfile();
//app.MapChangePassword();

app.Run();
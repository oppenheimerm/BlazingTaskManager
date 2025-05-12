using BlazingTaskManager.Client.AuthState;
using BlazingTaskManager.Client.Components;
using BlazingTaskManager.Client.Services;
using BlazingTaskManager.Client.ViewModels;
using BlazingTaskManager.Shared.Services.AuthService;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7015/") });

builder.Services.AddScoped<IJWTUtilities, JWTUtilities>();
builder.Services.AddScoped<IAccountLoginVM, AccountLoginVM>();
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<IPageUIService, PageUIService>();


builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<LocalStorageService>();
//  Ensure ProtectedLocalStorage is registered
builder.Services.AddScoped<ProtectedLocalStorage>();


//  - CustomAuthenticationStateProvider should be registered as Scoped in 
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();

builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();

builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Secret"]!))
    };
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// need to add so we can handle redirects i.e. 401, 404, etc...
app.UseStatusCodePagesWithReExecute("/");
app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

//  user authentication / authorization

app.UseAuthentication();
app.UseAuthorization();

app.Run();

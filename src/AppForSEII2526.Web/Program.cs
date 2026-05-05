using AppForSEII2526.Web;
using AppForSEII2526.Web.API;
using AppForSEII2526.Web.Components;
using AppForSEII2526.Web.Components.Account;
using AppForSEII2526.Web.Data;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = IdentityConstants.ApplicationScheme;
    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
})
    .AddIdentityCookies();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();
//incorporacion de ReviewStateContainer
builder.Services.AddScoped<ReviewStateContainer>();
//incorporacion de RentalStateContainer
builder.Services.AddScoped<RentalStateContainer>();
//incorporacion de PurchaseStateContainer
builder.Services.AddScoped<PurchaseStateContainer>();
//incorporacion del clientAPI
builder.Services.AddHttpClient(); // Registra el HttpClientFactory

builder.Services.AddScoped<AppForSEII2526APIClient>(services =>
{
    var configuration = services.GetRequiredService<IConfiguration>();
    var baseUrl = configuration["ApiSettings:BaseUrl"];
    var httpClientFactory = services.GetRequiredService<IHttpClientFactory>();
    var client = httpClientFactory.CreateClient();
    client.BaseAddress = new Uri(baseUrl);
    return new AppForSEII2526APIClient(baseUrl, client);
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

app.Run();

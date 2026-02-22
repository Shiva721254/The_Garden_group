using DotNetEnv;
using Microsoft.AspNetCore.Authentication.Cookies;
using MongoDB.Driver;
using The_Garden_Group.Data;
using The_Garden_Group.Repositories;
using The_Garden_Group.Services;

var builder = WebApplication.CreateBuilder(args);

// Load .env (must be before reading env vars)
Env.Load();

// MVC
builder.Services.AddControllersWithViews();

// Mongo settings from appsettings.json
builder.Services.Configure<MongoSettings>(builder.Configuration.GetSection("Mongo"));

// MongoClient (Singleton)
builder.Services.AddSingleton<IMongoClient>(_ =>
{
    var uri = Environment.GetEnvironmentVariable("MONGODB_URI");
    if (string.IsNullOrWhiteSpace(uri))
        throw new InvalidOperationException("Missing MONGODB_URI in .env");
    return new MongoClient(uri);
});

// IMongoDatabase (Scoped)
builder.Services.AddScoped<IMongoDatabase>(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    var settings = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<MongoSettings>>().Value;

    if (string.IsNullOrWhiteSpace(settings.Database))
        throw new InvalidOperationException("Mongo:Database missing in appsettings.json");

    return client.GetDatabase(settings.Database);
});

// Repositories + Services
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<ITicketRepository, TicketRepository>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<DashboardService>();
builder.Services.AddScoped<SeedService>();

// Cookie authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.AccessDeniedPath = "/Auth/Denied";
        options.Cookie.Name = "The_Garden_group.Auth";
    });

// Authorization policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("EmployeeOnly", p => p.RequireClaim("role", "employee"));
    options.AddPolicy("ServiceDeskOnly", p => p.RequireClaim("role", "serviceDesk"));
});

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<The_Garden_Group.Services.SeedService>();
    await seeder.EnsureAdminAsync();
}

app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Root URL -> Auth/Login
app.MapControllerRoute(
    name: "root",
    pattern: "",
    defaults: new { controller = "Auth", action = "Login" }
);

// Normal MVC routing -> controller/action (default action Index)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.Run();
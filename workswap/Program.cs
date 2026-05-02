using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Text;
using workswap.Data;
using workswap.Models;
using workswap.Services;

var builder = WebApplication.CreateBuilder(args);

// --- 1. CONFIGURATION ---
// Default builder already loads appsettings.json and environment variables.
// We explicitly add .env support via DotNetEnv above.

// --- 2. SERVICES ---
builder.Services.AddControllers();

// Built-in OpenAPI for .NET 10
builder.Services.AddOpenApi();

// Database Connection (Supabase PostgreSQL)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var isTesting = builder.Environment.IsEnvironment("Testing");

if (!isTesting)
{
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlite(connectionString));
}

// Identity Setup (User & Role Management)
builder.Services.AddIdentityCore<ApplicationUser>(options => {
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;
})
.AddRoles<IdentityRole<int>>()
.AddEntityFrameworkStores<ApplicationDbContext>();

// JWT Authentication Setup
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer();

// Configure JWT options after the host is built so test overrides are available
builder.Services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
    .Configure<IConfiguration>((options, configuration) =>
    {
        var jwtSecret = configuration["JWT_SECRET"]
            ?? throw new Exception("JWT_SECRET is missing from configuration!");

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = configuration["JWT_ISSUER"],
            ValidAudience = configuration["JWT_AUDIENCE"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
        };
    });

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000") // Next.js dev server
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.AddAuthorization();

// Custom Application Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IShiftService, ShiftService>();
builder.Services.AddScoped<ISwapService, SwapService>();
builder.Services.AddScoped<IShiftOfferService, ShiftOfferService>();
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<INotificationService, NotificationService>();

var app = builder.Build();

// --- 3. MIDDLEWARE PIPELINE ---
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

// CORS must be before Authentication/Authorization
app.UseCors("AllowFrontend");

// Authentication MUST come before Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// --- 4. DATABASE INITIALIZATION ---
// Apply migrations and seed data on startup
if (!app.Environment.IsEnvironment("Testing"))
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        try
        {
            var context = services.GetRequiredService<ApplicationDbContext>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole<int>>>();

            // Ensure database is created and migrations are applied
            if (context.Database.GetPendingMigrations().Any())
            {
                context.Database.Migrate();
            }

            // Seed initial roles and admin user
            await DatabaseSeeder.SeedAsync(userManager, roleManager);
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred during database initialization.");
        }
    }
}

app.Run();

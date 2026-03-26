using Microsoft.AspNetCore.Identity;
using workswap.Models;

namespace workswap.Data;

/// <summary>
/// This class helps us seed initial data like Roles and a default Admin user.
/// </summary>
public static class DatabaseSeeder
{
    public static async Task SeedAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole<int>> roleManager)
    {
        // 1. Create Roles if they don't exist
        string[] roles = { "Admin", "Manager", "Employee" };

        foreach (var roleName in roles)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole<int>(roleName));
            }
        }

        // 2. Create Default Admin User
        var adminEmail = "admin@workswap.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FirstName = "System",
                LastName = "Admin",
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(adminUser, "Admin@123");

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }
    }
}

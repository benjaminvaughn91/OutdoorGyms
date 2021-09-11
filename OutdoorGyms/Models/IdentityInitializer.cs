using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace OutdoorGyms.Models
{
    public class IdentityInitializer
    {
        public static async Task EnsurePopulated(IServiceProvider services)
        {
            var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            await CreateRoles(roleManager);
            //await CreateDummyUsers(userManager);
            //await CreateAdmin(services);
        }

        private static async Task CreateRoles(RoleManager<IdentityRole> rManager)
        {
            if (!await rManager.RoleExistsAsync("Administrator"))
            {
                await rManager.CreateAsync(new IdentityRole("Administrator"));
            }
            if (!await rManager.RoleExistsAsync("Moderator"))
            {
                await rManager.CreateAsync(new IdentityRole("Moderator"));
            }
            if (!await rManager.RoleExistsAsync("Contributor"))
            {
                await rManager.CreateAsync(new IdentityRole("Contributor"));
            }
        }

        private static async Task CreateDummyUsers(UserManager<IdentityUser> uManager)
        {
            IdentityUser E100 = new IdentityUser("E100");
            IdentityUser E101 = new IdentityUser("E101");
            IdentityUser E102 = new IdentityUser("E102");
            IdentityUser E103 = new IdentityUser("E103");
            IdentityUser E200 = new IdentityUser("E200");
            IdentityUser E201 = new IdentityUser("E201");
            IdentityUser E202 = new IdentityUser("E202");
            IdentityUser E203 = new IdentityUser("E203");
            IdentityUser E300 = new IdentityUser("E300");
            IdentityUser E301 = new IdentityUser("E301");
            IdentityUser E302 = new IdentityUser("E302");
            IdentityUser E303 = new IdentityUser("E303");
            IdentityUser E400 = new IdentityUser("E400");
            IdentityUser E401 = new IdentityUser("E401");
            IdentityUser E402 = new IdentityUser("E402");
            IdentityUser E403 = new IdentityUser("E403");
            IdentityUser E500 = new IdentityUser("E500");
            IdentityUser E501 = new IdentityUser("E501");
            IdentityUser E502 = new IdentityUser("E502");
            IdentityUser E503 = new IdentityUser("E503");
            IdentityUser E901 = new IdentityUser("E901");
            IdentityUser E902 = new IdentityUser("E902");
            IdentityUser E774 = new IdentityUser("E774");
            IdentityUser E700 = new IdentityUser("E700");

            await uManager.CreateAsync(E100, "Pass02?");
            await uManager.CreateAsync(E101, "Pass03?");
            await uManager.CreateAsync(E102, "Pass04?");
            await uManager.CreateAsync(E103, "Pass05?");
            await uManager.CreateAsync(E200, "Pass06?");
            await uManager.CreateAsync(E201, "Pass07?");
            await uManager.CreateAsync(E202, "Pass08?");
            await uManager.CreateAsync(E203, "Pass09?");
            await uManager.CreateAsync(E300, "Pass10?");
            await uManager.CreateAsync(E301, "Pass11?");
            await uManager.CreateAsync(E302, "Pass12?");
            await uManager.CreateAsync(E303, "Pass13?");
            await uManager.CreateAsync(E400, "Pass14?");
            await uManager.CreateAsync(E401, "Pass15?");
            await uManager.CreateAsync(E402, "Pass16?");
            await uManager.CreateAsync(E403, "Pass17?");
            await uManager.CreateAsync(E500, "Pass18?");
            await uManager.CreateAsync(E501, "Pass19?");
            await uManager.CreateAsync(E502, "Pass20?");
            await uManager.CreateAsync(E503, "Pass21?");
            await uManager.CreateAsync(E901, "Pass21?");
            await uManager.CreateAsync(E902, "Pass21?");
            await uManager.CreateAsync(E774, "Pass27?");
            await uManager.CreateAsync(E700, "Pass28?");

            await uManager.AddToRoleAsync(E100, "Moderator");
            await uManager.AddToRoleAsync(E101, "Contributor");
            await uManager.AddToRoleAsync(E102, "Contributor");
            await uManager.AddToRoleAsync(E103, "Contributor");
            await uManager.AddToRoleAsync(E200, "Moderator");
            await uManager.AddToRoleAsync(E201, "Contributor");
            await uManager.AddToRoleAsync(E202, "Contributor");
            await uManager.AddToRoleAsync(E203, "Contributor");
            await uManager.AddToRoleAsync(E300, "Moderator");
            await uManager.AddToRoleAsync(E301, "Contributor");
            await uManager.AddToRoleAsync(E302, "Contributor");
            await uManager.AddToRoleAsync(E303, "Contributor");
            await uManager.AddToRoleAsync(E400, "Moderator");
            await uManager.AddToRoleAsync(E401, "Contributor");
            await uManager.AddToRoleAsync(E402, "Contributor");
            await uManager.AddToRoleAsync(E403, "Contributor");
            await uManager.AddToRoleAsync(E500, "Moderator");
            await uManager.AddToRoleAsync(E501, "Contributor");
            await uManager.AddToRoleAsync(E502, "Contributor");
            await uManager.AddToRoleAsync(E503, "Contributor");
            await uManager.AddToRoleAsync(E901, "Contributor");
            await uManager.AddToRoleAsync(E902, "Contributor");
            await uManager.AddToRoleAsync(E774, "Contributor");
            await uManager.AddToRoleAsync(E700, "Contributor");
        }

        private static async Task CreateAdmin(IServiceProvider services)
        {
            var uModerator = services.GetRequiredService<UserManager<IdentityUser>>();
            IdentityUser A000 = new IdentityUser("A000");
            await uModerator.CreateAsync(A000, "password");
            await uModerator.AddToRoleAsync(A000, "Administrator");
        }

    }
}

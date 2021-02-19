using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace EnvironmentCrime.Models
{
    /*Contains methods for managing the IdentityUser accounts.*/
    public class AccountManager
    {
        static UserManager<IdentityUser> uManager;

        public static void Initialize(IServiceProvider services)
        {
            uManager = services.GetRequiredService<UserManager<IdentityUser>>();
        }

        /*Creates a new IdentityUser and sets its Role.*/
        public static async Task CreateUser(string userName, string password, string roleTitle)
        { 
            IdentityUser user = new IdentityUser(userName);
            try
            {
                await uManager.CreateAsync(user, password);
            }
            catch(Exception e) { Debug.Print(e.Message);  }

            await uManager.AddToRoleAsync(user, roleTitle);
        }

        /*Updates the password and changes the Role of a specified IdentityUser.*/
        public static async Task UpdateUser(string userName, string currentPassword, string newPassword, string currentRole, string newRole)
        {
            var user = await uManager.FindByNameAsync(userName);

            await uManager.ChangePasswordAsync(user, currentPassword, newPassword);
            await uManager.RemoveFromRoleAsync(user, currentRole);
            await uManager.AddToRoleAsync(user, newRole);
        }

        /*Removes the IdentityUser with the specified userId.*/
        public static async Task RemoveUser(string userId)
        {
            var user = await uManager.FindByNameAsync(userId);
            await uManager.DeleteAsync(user);
        }
    }
}

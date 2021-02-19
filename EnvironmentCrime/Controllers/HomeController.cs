using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using EnvironmentCrime.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SessionTest.Infrastructure;

namespace EnvironmentCrime.Controllers
{
    /*Controller class with an Action for each View in the Home-folder.*/
    [Authorize]
    public class HomeController : Controller
    {
        private UserManager<IdentityUser> userManager;
        private SignInManager<IdentityUser> signInManager;
        private ICrimeRepository repository;

        public HomeController(UserManager<IdentityUser> userMgr,
        SignInManager<IdentityUser> signInMgr, ICrimeRepository repo)
        {
            userManager = userMgr;
            signInManager = signInMgr;
            repository = repo;
        }

        [AllowAnonymous]
        public ViewResult Index()
        {
            ViewBag.Title = "Småstads Kommun";

            var myErrand = HttpContext.Session.GetJson<Errand>("NewErrand");
            if (myErrand == null)
            {
                return View();
            }
            else
            {
                HttpContext.Session.Remove("NewErrand");
                return View(myErrand);
            }
        }

        [AllowAnonymous]
        public ViewResult Login(string returnUrl)
        {
            ViewBag.Title = "Logga in:Småstads Kommun";

            return View(new LoginModel
            {
                ReturnUrl = returnUrl
            });
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel loginModel)
        {
            IdentityUser user = await userManager.FindByNameAsync(loginModel.UserName);
            if (ModelState.IsValid)
            {
                if (user != null)
                {
                    await signInManager.SignOutAsync(); //om personen är inloggad så tar vi bort sessionen
                    if ((await signInManager.PasswordSignInAsync(user, loginModel.Password, false, false)).Succeeded)
                    {
                        if (await userManager.IsInRoleAsync(user, "Administrator"))
                        {
                            return Redirect("/Administrator/StartAdministrator");
                        }
                        if (await userManager.IsInRoleAsync(user, "Coordinator")) 
                        {
                            return Redirect("/Coordinator/StartCoordinator");
                        }
                        if (await userManager.IsInRoleAsync(user, "Manager")) 
                        {
                            return Redirect("/Manager/StartManager");
                        }
                        if (await userManager.IsInRoleAsync(user, "Investigator"))
                        {
                            return Redirect("/Investigator/StartInvestigator");
                        }
                    }
                }
            }
            ModelState.AddModelError("", "Felaktigt användarnamn eller lösenord");
            return View(loginModel);
        }

        public async Task<RedirectResult> Logout(string returnUrl = "/")
        {
            await signInManager.SignOutAsync();
            return Redirect(returnUrl);
        }

        [AllowAnonymous]
        public ViewResult AccessDenied()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult NewUser()
        {
            string userName = "E889";
            string password = "Pass89?";
            string roleTitle = "Investigator";
            Employee employee = new Employee
            { 
                EmployeeId = userName,
                EmployeeName = userName,
                DepartmentId = "D05",
                RoleTitle = roleTitle
            };
            if (repository.SaveEmployee(employee))
            {
                AccountManager.CreateUser(userName, password, roleTitle).Wait();
            }
            else
            {
                Debug.Print("already exists");
            }
            return RedirectToAction("Login");
        }
    }
}

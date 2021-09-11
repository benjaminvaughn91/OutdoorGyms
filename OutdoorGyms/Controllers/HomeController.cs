using System.Collections.Generic;
using System.Threading.Tasks;
using OutdoorGyms.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SessionTest.Infrastructure;

namespace OutdoorGyms.Controllers
{
    /*Controller class with an Action for each View in the Home-folder.*/
    [Authorize]
    public class HomeController : Controller
    {
        private UserManager<IdentityUser> userManager;
        private SignInManager<IdentityUser> signInManager;
        private IGymRepository repository;

        public HomeController(UserManager<IdentityUser> userMgr,
        SignInManager<IdentityUser> signInMgr, IGymRepository repo)
        {
            userManager = userMgr;
            signInManager = signInMgr;
            repository = repo;
        }

        [AllowAnonymous]
        public ViewResult Index()
        {
            ViewBag.Title = "OutdoorGyms";

            List<Gym> gyms = new List<Gym>();
            foreach (Gym gym in GetGymsToShow())
                if (gym.StatusId == "S_B")
                    gyms.Add(gym);
            ViewBag.GymList = gyms;

            return View(repository);
        }

        [AllowAnonymous]
        public ViewResult Login(string returnUrl)
        {
            ViewBag.Title = "LogIn:OutdoorGyms";

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
                        if (await userManager.IsInRoleAsync(user, "Moderator")) 
                        {
                            return Redirect("/Moderator/StartModerator");
                        }
                        if (await userManager.IsInRoleAsync(user, "Contributor"))
                        {
                            return Redirect("/Contributor/StartContributor");
                        }
                    }
                }
            }
            ModelState.AddModelError("", "Wrong username or password.");
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

        /*Saves a List of Gyms to a session that are filtered by either status or county.*/
        [AllowAnonymous]
        public IActionResult FilterGyms(string countyid)
        {
            List<Gym> gyms = new List<Gym>();
            if (countyid == "all")
                return RedirectToAction("Index");
            else
            {
                foreach (Gym gym in repository.Gyms)
                {
                    if (gym.CountyId == countyid)
                        gyms.Add(gym);
                }
            }
            HttpContext.Session.SetJson("FilteredGymList", gyms);
            return RedirectToAction("Index");
        }

        /*Saves a List of Gyms of which the Reference Number matches the specified casenumber.*/
        [AllowAnonymous]
        public IActionResult SearchGyms(string casenumber)
        {
            List<Gym> gyms = new List<Gym>();
            foreach (Gym gym in repository.Gyms)
            {
                if (gym.RefNumber == casenumber)
                    gyms.Add(gym);
            }
            HttpContext.Session.SetJson("SearchedGymList", gyms);
            return RedirectToAction("Index");
        }

        /*Returns the list of Gyms to be displayed. Gyms are retrieved from either session FilteredGymList
        or SearchedGymList, depending on which Action the user called. If neither exists it will be the same
        Gyms as in repository.Gyms.*/
        List<Gym> GetGymsToShow()
        {
            var gyms = HttpContext.Session.GetJson<List<Gym>>("FilteredGymList");
            if (gyms != null)
                HttpContext.Session.Remove("FilteredGymList");
            else
            {
                gyms = HttpContext.Session.GetJson<List<Gym>>("SearchedGymList");
                if (gyms != null)
                    HttpContext.Session.Remove("SearchedGymList");
                else
                {
                    gyms = new List<Gym>();
                    foreach (Gym gym in repository.Gyms) gyms.Add(gym);
                }
            }
            return gyms;
        }
    }
}

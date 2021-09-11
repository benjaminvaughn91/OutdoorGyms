using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using OutdoorGyms.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SessionTest.Infrastructure;


namespace OutdoorGyms.Controllers
{
    /*Controller class with an Action for each View in the Contributor-folder. Authorized for this role only.*/
    [Authorize(Roles = "Contributor")]
    public class ContributorController : Controller
    {
        private IGymRepository repository; 
        private IWebHostEnvironment environment;

        public ContributorController(IGymRepository repo, IWebHostEnvironment env)
        {
            repository = repo;
            environment = env;
        }
        public ViewResult GymContributor(string id)
        {
            ViewBag.Title = "Gym:Contributor:OutdoorGyms";
            ViewBag.ID = id;

            List<GymStatus> statusList = new List<GymStatus>();
            statusList = repository.GymStatuses.Where(item => item.StatusId == "S_C" || item.StatusId == "S_D").ToList();
            ViewBag.ListOfStatuses = statusList;

            var err = repository.GetGymDetail(int.Parse(id));
            HttpContext.Session.SetJson("UpdatedGym", err);

            return View();
        }
        public ViewResult AddGym()
        {
            ViewBag.Title = "Add:Contributor:OutdoorGyms";

            //Send a list of countys that does not contain "OutdoorGyms" for dropdown menu
            ViewBag.ListOfCountys = repository.Countys.
                Where(item => item.CountyName != "OutdoorGyms").ToList();

            var myGym = HttpContext.Session.GetJson<Gym>("NewGym");
            if (myGym == null)
            {
                return View();
            }
            else
            {
                HttpContext.Session.Remove("NewGym");
                return View(myGym);
            }
        }
        public ViewResult Validate(Gym gym, IFormFile loadImage)
        {
            ViewBag.Title = "Confirm:Contributor:OutdoorGyms";
            ViewBag.County = repository.GetGymCounty(gym);
            ViewBag.Town = gym.Town;
            ViewBag.Place = gym.Place;
            ViewBag.Description = gym.Description;

            gym.DateOfContribution = DateTime.Today;
            gym.EmployeeId = repository.GetCurrentEmployeeID();
            gym.ContributorName = repository.GetCurrentEmployee().EmployeeName;

            HttpContext.Session.SetJson("NewGym", gym);
            HttpContext.Session.SetJson("ContributorName", gym);

            return View(gym);
        }
        public ViewResult Thanks()
        {
            ViewBag.Title = "Thanks:Contributor:OutdoorGyms";
            Gym gym = HttpContext.Session.GetJson<Gym>("NewGym");
            repository.SaveGym(gym);

            ViewBag.RefNumber = gym.RefNumber;
            HttpContext.Session.Remove("NewGym");
            return View();
        }
        public ViewResult StartContributor()
        {
            ViewBag.Title = "Contributor:OutdoorGyms";

            List<Gym> gyms = new List<Gym>();
            foreach (Gym gym in GetGymsToShow())
                if (gym.StatusId == "S_B")
                    gyms.Add(gym);
            ViewBag.GymList = gyms;

            return View(repository);
        }

        /*Called in view GymContributor when the user clicks the save button.
        Gets the displayed Gym from a session, changes some properties and sends it to repository to save it.
        Also uploads a Picture and/or a Sample by calling UploadFiles.*/
        public IActionResult UpdateGym(string information, IFormFile loadImage)
        {
            var gym = HttpContext.Session.GetJson<Gym>("UpdatedGym");
            if (gym != null)
            {
                if (information != null)
                    gym.Description += System.Environment.NewLine 
                        + System.Environment.NewLine + information;

                if (loadImage != null)
                {
                    Task<string> fileName = UploadFiles(loadImage, "pictures");
                    Picture newPicture = new Picture();
                    newPicture.PictureName = fileName.Result;
                    newPicture.GymId = gym.GymId;
                    repository.SavePicture(newPicture);
                }
                repository.UpdateGym(gym);
            }
            HttpContext.Session.Remove("UpdatedGym");
            return RedirectToAction("GymContributor", new { id = gym.GymId });
        }

        /*Takes the IFormFile object and the name of the folder to save it in (either "pictures" or "samples")
         Saves it and returns the generated filename. Returns a string which be the Name of a new Picture or Sample object.*/
        public async Task<string> UploadFiles(IFormFile loadImage, string subFolder)
        {
            var tempPath = Path.GetTempFileName();
            string uniqueFileName = "";
            if (loadImage.Length > 0)
            {
                using (var stream = new FileStream(tempPath, FileMode.Create))
                {
                    await loadImage.CopyToAsync(stream);
                }

                uniqueFileName = Guid.NewGuid().ToString() + "_" + loadImage.FileName;

                var path = Path.Combine(environment.WebRootPath, "uploads/" + subFolder, uniqueFileName);

                System.IO.File.Move(tempPath, path);
            }
            return uniqueFileName;
         }

        /*Saves a List of Gyms (to a session) that are filtered by status.*/
        public IActionResult FilterGyms(string countyid, string mineid)
        {
            List<Gym> gyms = new List<Gym>();
            if (mineid == "all" && countyid == "all")
                return RedirectToAction("StartContributor");
            else
            {
                if (mineid != "all" && countyid != "all")
                    foreach (Gym gym in repository.Gyms)
                    {
                        if (gym.EmployeeId == repository.GetCurrentEmployeeID() && gym.CountyId == countyid)
                            gyms.Add(gym);
                    }
                else if (countyid == "all")
                {
                    Debug.Print("filtergym2s");
                    foreach (Gym gym in repository.Gyms)
                    {
                        if (gym.EmployeeId == repository.GetCurrentEmployeeID())
                        {
                            gyms.Add(gym);
                        }
                    }
                }
                else if (mineid == "all")
                    foreach (Gym gym in repository.Gyms)
                    {
                        if (gym.CountyId == countyid)
                            gyms.Add(gym);
                    }
                HttpContext.Session.SetJson("FilteredGymList", gyms);
                return RedirectToAction("StartContributor");
            }
        }

        /*Saves a List of Gyms of which the Reference Number matches the specified casenumber.*/
        public IActionResult SearchGyms(string casenumber)
        {
            List<Gym> gyms = new List<Gym>();
            foreach (Gym gym in repository.Gyms)
            {
                if (gym.RefNumber == casenumber)
                    gyms.Add(gym);
            }
            HttpContext.Session.SetJson("SearchedGymList", gyms);
            return RedirectToAction("StartContributor");
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

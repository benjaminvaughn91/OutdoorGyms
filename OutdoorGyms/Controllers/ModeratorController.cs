using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OutdoorGyms.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SessionTest.Infrastructure;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace OutdoorGyms.Controllers
{
    /*Controller class with an Action for each View in the Moderator-folder. Authorized for this role only.*/
    [Authorize(Roles = "Moderator")]
    public class ModeratorController : Controller
    {
        private IGymRepository repository;
        private IWebHostEnvironment environment;

        public ModeratorController(IGymRepository repo, IWebHostEnvironment env)
        {
            repository = repo;
            environment = env;
        }
        public ViewResult GymModerator(string id)
        {
            ViewBag.Title = "Gym:Moderator:OutdoorGyms";
            ViewBag.ID = id;
            ViewBag.ListOfStatuses = repository.GymStatuses;

            var gym = repository.GetGymDetail(int.Parse(id));
            ViewBag.Description = gym.Description;
            HttpContext.Session.SetJson("UpdatedGym", gym);

            return View();
        }
        public ViewResult StartModerator()
        {
            ViewBag.Title = "Moderator:OutdoorGyms";

            //Show only Gyms that belong to the same County as the manager
            List<Gym> gymsInCounty = new List<Gym>();
            Employee currentEmp = repository.GetCurrentEmployee();
            foreach (Gym gym in GetGymsToShow())
                gymsInCounty.Add(gym);

            ViewBag.GymList = gymsInCounty;
            ViewBag.ListOfEmployees = GetEmployeesForCounty(currentEmp);

            return View(repository);
        }

        /*Used for the drop-down menus. Returns the Employees that belong to the current managers county.*/
        public List<Employee> GetEmployeesForCounty(Employee currentEmployee)
        {
            List<Employee> employeeList = new List<Employee>();
            foreach (Employee employee in repository.Employees)
            {
                if (employee.CountyId == currentEmployee.CountyId)
                    employeeList.Add(employee);
            }
            return employeeList;
        }

        /*Called in view GymModerator when the user clicks the save button.
        Gets the displayed Gym from a session, changes some properties and sends it to repository to save it*/
        public IActionResult UpdateGym(string StatusId, IFormFile loadImage, string information)
        {
            var gym = HttpContext.Session.GetJson<Gym>("UpdatedGym");
            if (gym != null)
            {
                if (StatusId != "nothing")
                    gym.StatusId = StatusId;

                if (information != null)
                    gym.Description = information;

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
            return RedirectToAction("GymModerator", new { id = gym.GymId });
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

        /*Saves a List of Gyms (to a session) that are filtered by either status or employees (investigators).*/
        public IActionResult FilterGyms(string statusid, string countyid)
        {
            List<Gym> gyms = new List<Gym>();
            if (statusid == "all" && countyid == "all")
                return RedirectToAction("StartModerator");
            else
            {
                if (statusid != "all" && countyid != "all")
                    foreach (Gym gym in repository.Gyms)
                    {
                        if (gym.StatusId == statusid && gym.CountyId == countyid)
                            gyms.Add(gym);
                    }
                else if (countyid == "all")
                    foreach (Gym gym in repository.Gyms)
                    {
                        if (gym.StatusId == statusid)
                            gyms.Add(gym);
                    }
                else if (statusid == "all")
                    foreach (Gym gym in repository.Gyms)
                    {
                        if (gym.CountyId == countyid)
                            gyms.Add(gym);
                    }
                HttpContext.Session.SetJson("FilteredGymList", gyms);
                return RedirectToAction("StartModerator");
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
            return RedirectToAction("StartModerator");
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

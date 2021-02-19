using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EnvironmentCrime.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SessionTest.Infrastructure;


namespace EnvironmentCrime.Controllers
{
    /*Controller class with an Action for each View in the Investigator-folder. Authorized for this role only.*/
    [Authorize(Roles = "Investigator")]
    public class InvestigatorController : Controller
    {
        private ICrimeRepository repository; 
        private IWebHostEnvironment environment;

        public InvestigatorController(ICrimeRepository repo, IWebHostEnvironment env)
        {
            repository = repo;
            environment = env;
        }
        public ViewResult CrimeInvestigator(string id)
        {
            ViewBag.Title = "Ärenden:Handläggare:Småstads Kommun";
            ViewBag.ID = id;

            List<ErrandStatus> statusList = new List<ErrandStatus>();
            statusList = repository.ErrandStatuses.Where(item => item.StatusId == "S_C" || item.StatusId == "S_D").ToList();
            ViewBag.ListOfStatuses = statusList;

            var err = repository.GetErrandDetail(int.Parse(id));
            HttpContext.Session.SetJson("UpdatedErrand", err);

            return View();
        }
        public ViewResult StartInvestigator()
        {
            ViewBag.Title = "Handläggare:Småstads Kommun";

            //Show only the Errands that belong to this user
            List<Errand> errandList = new List<Errand>();
            foreach (Errand errand in GetErrandsToShow())
            {
                if (errand.EmployeeId == repository.GetCurrentEmployee().EmployeeId)
                    errandList.Add(errand);
            }
            ViewBag.ErrandList = errandList;

            return View(repository);
        }

        /*Called in view CrimeInvestigator when the user clicks the save button.
        Gets the displayed Errand from a session, changes some properties and sends it to repository to save it.
        Also uploads a Picture and/or a Sample by calling UploadFiles.*/
        public IActionResult UpdateErrand(string statusId, string events, string information, IFormFile loadImage, IFormFile loadSample)
        {
            var err = HttpContext.Session.GetJson<Errand>("UpdatedErrand");
            if (err != null)
            {
                if (statusId != "nothing") err.StatusId = statusId;
                err.InvestigatorAction += " " + events;
                err.InvestigatorInfo += " " + information;
                
                if (loadImage != null)
                {
                    Task<string> fileName = UploadFiles(loadImage, "pictures");
                    Picture newPicture = new Picture();
                    newPicture.PictureName = fileName.Result;
                    newPicture.ErrandId = err.ErrandId;
                    repository.SavePicture(newPicture);
                }
                if (loadSample != null)
                {
                    Task<string> fileName = UploadFiles(loadSample, "samples");
                    Sample newSample = new Sample();
                    newSample.SampleName = fileName.Result;
                    newSample.ErrandId = err.ErrandId;
                    repository.SaveSample(newSample);
                }
                repository.UpdateErrand(err);
            }
            HttpContext.Session.Remove("UpdatedErrand");
            return RedirectToAction("CrimeInvestigator", new { id = err.ErrandId });
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

        /*Saves a List of Errands (to a session) that are filtered by status.*/
        public IActionResult FilterErrands(string statusid)
        {
            List<Errand> errands = new List<Errand>();
            if (statusid == "all")
                return RedirectToAction("StartInvestigator");
            else
            {
                foreach (Errand errand in repository.Errands)
                {
                    if (errand.StatusId == statusid)
                        errands.Add(errand);
                }
                HttpContext.Session.SetJson("FilteredErrandList", errands);
                return RedirectToAction("StartInvestigator");
            }
        }

        /*Saves a List of Errands of which the Reference Number matches the specified casenumber.*/
        public IActionResult SearchErrands(string casenumber)
        {
            List<Errand> errands = new List<Errand>();
            foreach (Errand errand in repository.Errands)
            {
                if (errand.RefNumber == casenumber)
                    errands.Add(errand);
            }
            HttpContext.Session.SetJson("SearchedErrandList", errands);
            return RedirectToAction("StartInvestigator");
        }

        /*Returns the list of Errands to be displayed. Errands are retrieved from either session FilteredErrandList
        or SearchedErrandList, depending on which Action the user called. If neither exists it will be the same
        Errands as in repository.Errands.*/
        List<Errand> GetErrandsToShow()
        {
            var errands = HttpContext.Session.GetJson<List<Errand>>("FilteredErrandList");
            if (errands != null)
                HttpContext.Session.Remove("FilteredErrandList");
            else
            {
                errands = HttpContext.Session.GetJson<List<Errand>>("SearchedErrandList");
                if (errands != null)
                    HttpContext.Session.Remove("SearchedErrandList");
                else
                {
                    errands = new List<Errand>();
                    foreach (Errand errand in repository.Errands) errands.Add(errand);
                }
            }
            return errands;
        }
    }
}

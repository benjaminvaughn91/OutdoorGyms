using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using EnvironmentCrime.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SessionTest.Infrastructure;

namespace EnvironmentCrime.Controllers
{
    /*Controller class with an Action for each View in the Coordinator-folder. Authorized for this role only.*/
    [Authorize(Roles = "Coordinator")]
    public class CoordinatorController : Controller
    {
        private ICrimeRepository repository;
        public CoordinatorController(ICrimeRepository repo)
        {
            repository = repo;
        }
        public ViewResult CrimeCoordinator(string id)
        {
            ViewBag.Title = "Ärenden:Samordnare:Småstads Kommun";
            ViewBag.ID = id;

            //Send a list of departments that does not contain "Småstads Kommun" for dropdown menu
            ViewBag.ListOfDepartments = repository.Departments.
                Where(item => item.DepartmentName != "Småstads Kommun").ToList();

            var err = repository.GetErrandDetail(int.Parse(id));
            HttpContext.Session.SetJson("UpdatedErrand", err);

            return View();
        }
        public ViewResult ReportCrime()
        {
            ViewBag.Title = "Rapportera:Samordnare:Småstads Kommun";
            
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
        public ViewResult StartCoordinator()
        {
            ViewBag.Title = "Samordnare:Småstads Kommun";

            ViewBag.ErrandList = GetErrandsToShow();

            //Send a list of departments that does not contain "Småstads Kommun" for dropdown menu
            ViewBag.ListOfDepartments = repository.Departments.
                Where(item => item.DepartmentName != "Småstads Kommun").ToList();

            return View(repository);
        }
        public ViewResult Thanks()
        {
            ViewBag.Title = "Tack:Samordnare:Småstads Kommun";
            Errand err = HttpContext.Session.GetJson<Errand>("NewErrand");
            repository.SaveErrand(err);
            ViewBag.RefNumber = err.RefNumber;
            HttpContext.Session.Remove("NewErrand");
            return View();
        }
        public ViewResult Validate(Errand errand)
        {
            ViewBag.Title = "Bekräftelse:Samordnare:Småstads Kommun";
            ViewBag.Place = errand.Place;
            ViewBag.TypeOfCrime = errand.TypeOfCrime;
            ViewBag.DateOfObservation = errand.DateOfObservation;
            ViewBag.InformerName = errand.InformerName;
            ViewBag.InformerPhone = errand.InformerPhone;
            ViewBag.Observation = errand.Observation;
            
            HttpContext.Session.SetJson("NewErrand", errand);
            return View(errand);
        }

        /*Called in view CrimeCoordinator when the user clicks the save button.
         Gets the displayed Errand from a session, changes the DepartmentId and sends it to repository to save it*/
        public IActionResult UpdateErrand(string departmentId)
        {
            var err = HttpContext.Session.GetJson<Errand>("UpdatedErrand");
            if (departmentId != "nothing")
            {
                if (err != null)
                {
                    err.DepartmentId = departmentId;
                    repository.UpdateErrand(err);
                }
            }
            HttpContext.Session.Remove("UpdatedErrand");
            return RedirectToAction("CrimeCoordinator", new { id = err.ErrandId });
        }

        /*Saves a List of Errands (to a session) that are filtered by either status or department.*/
        public IActionResult FilterErrands(string statusid, string departmentid)
        {
            List<Errand> errands = new List<Errand>();
            if (statusid == "all" && departmentid == "all")
                return RedirectToAction("StartCoordinator");
            else
            {
                if (statusid != "all" && departmentid != "all")
                    foreach (Errand errand in repository.Errands) { 
                        if (errand.StatusId == statusid && errand.DepartmentId == departmentid)
                            errands.Add(errand);
                    }
                else if (departmentid == "all")
                    foreach (Errand errand in repository.Errands) { 
                        if (errand.StatusId == statusid)
                            errands.Add(errand);
                    }
                else if (statusid == "all")
                    foreach (Errand errand in repository.Errands) { 
                        if (errand.DepartmentId == departmentid)
                            errands.Add(errand);
                    }
                HttpContext.Session.SetJson("FilteredErrandList", errands);
                return RedirectToAction("StartCoordinator");
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
            return RedirectToAction("StartCoordinator");
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

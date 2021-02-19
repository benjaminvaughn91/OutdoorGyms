using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnvironmentCrime.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SessionTest.Infrastructure;

namespace EnvironmentCrime.Controllers
{
    /*Controller class with an Action for each View in the Manager-folder. Authorized for this role only.*/
    [Authorize(Roles = "Manager")]
    public class ManagerController : Controller
    {
        private ICrimeRepository repository;

        public ManagerController(ICrimeRepository repo)
        {
            repository = repo;
        }
        public ViewResult CrimeManager(string id)
        {
            ViewBag.Title = "Ärenden:Chef:Småstads Kommun";
            ViewBag.ID = id;

            ViewBag.ListOfEmployees = GetEmployeesForDepartment(repository.GetCurrentEmployee());

            var err = repository.GetErrandDetail(int.Parse(id));
            HttpContext.Session.SetJson("UpdatedErrand", err);

            return View();
        }
        public ViewResult StartManager()
        {
            ViewBag.Title = "Chef:Småstads Kommun";

            //Show only Errands that belong to the same Department as the manager
            List<Errand> errandsInDepartment = new List<Errand>();
            Employee currentEmp = repository.GetCurrentEmployee();
            foreach (Errand errand in GetErrandsToShow())
            {
                if (errand.DepartmentId == currentEmp.DepartmentId)
                    errandsInDepartment.Add(errand);
            }
            ViewBag.ErrandList = errandsInDepartment;

            ViewBag.ListOfEmployees = GetEmployeesForDepartment(currentEmp);

            return View(repository);
        }

        /*Used for the drop-down menus. Returns the Employees that belong to the current managers department.*/
        public List<Employee> GetEmployeesForDepartment(Employee currentEmployee)
        {
            List<Employee> employeeList = new List<Employee>();
            foreach (Employee employee in repository.Employees)
            {
                if (employee.DepartmentId == currentEmployee.DepartmentId)
                    employeeList.Add(employee);
            }
            return employeeList;
        }

        /*Called in view CrimeManager when the user clicks the save button.
        Gets the displayed Errand from a session, changes some properties and sends it to repository to save it*/
        public IActionResult UpdateErrand(string employeeId, bool noAction, string reason)
        {
            var err = HttpContext.Session.GetJson<Errand>("UpdatedErrand");
            if (err != null)
            {
                if (noAction)
                {
                    err.StatusId = "S_B";
                    err.InvestigatorInfo = reason;
                    err.EmployeeId = "";
                }
                else if (employeeId != "nothing")
                {
                    err.EmployeeId = employeeId;
                }
                repository.UpdateErrand(err);
            }
            HttpContext.Session.Remove("UpdatedErrand");
            return RedirectToAction("CrimeManager", new { id = err.ErrandId });
        }

        /*Saves a List of Errands (to a session) that are filtered by either status or employees (investigators).*/
        public IActionResult FilterErrands(string statusid, string investigatorid)
        {
            List<Errand> errands = new List<Errand>();
            if (statusid == "all" && investigatorid == "all")
                return RedirectToAction("StartManager");
            else
            {
                if (statusid != "all" && investigatorid != "all")
                    foreach (Errand errand in repository.Errands)
                    {
                        if (errand.StatusId == statusid && errand.EmployeeId == investigatorid)
                            errands.Add(errand);
                    }
                else if (investigatorid == "all")
                    foreach (Errand errand in repository.Errands)
                    {
                        if (errand.StatusId == statusid)
                            errands.Add(errand);
                    }
                else if (statusid == "all")
                    foreach (Errand errand in repository.Errands)
                    {
                        if (errand.EmployeeId == investigatorid)
                            errands.Add(errand);
                    }
                HttpContext.Session.SetJson("FilteredErrandList", errands);
                return RedirectToAction("StartManager");
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
            return RedirectToAction("StartManager");
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

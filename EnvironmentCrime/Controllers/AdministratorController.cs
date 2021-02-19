using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EnvironmentCrime.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using SessionTest.Infrastructure;

namespace EnvironmentCrime.Controllers
{
    /*Controller class with an Action for each View in the Administrator-folder. Authorized for this role only.*/
    [Authorize(Roles = "Administrator")]
    public class AdministratorController : Controller
    {
        private ICrimeRepository repository;

        public AdministratorController(ICrimeRepository repo, IWebHostEnvironment env)
        {
            repository = repo;
        }

        public ViewResult StartAdministrator()
        {
            ViewBag.Title = "Administratör:Småstads Kommun";

            List<Employee> employees = GetEmployeesToShow();
            ViewBag.EmployeeList = employees;
            ViewBag.HasEmployees = (employees.Count > 0) ? true : false;

            return View(repository);
        }
        public ViewResult EditUser(string id)
        {
            ViewBag.Title = "Redigera:Administratör:Småstads Kommun";
            ViewBag.ListOfDepartments = repository.Departments;

            var message = "";
            message = HttpContext.Session.GetJson<string>("ResultMessage");
            ViewBag.ResultMessage = message;

            var previousData = HttpContext.Session.GetJson<Employee>("PreviousEmployeeData");

            HttpContext.Session.Remove("ResultMessage");
            HttpContext.Session.Remove("PreviousEmployeeData");

            var employeeDetail = repository.Employees.Where(te => te.EmployeeId == id).FirstOrDefault();

            ViewBag.DepartmentName = repository.GetEmployeeDepartment(employeeDetail);

            HttpContext.Session.SetJson("PreviousEmployeeData", employeeDetail);

            return View(employeeDetail);
        }
        public ViewResult AddUser(string message = "")
        {
            ViewBag.Title = "Läggtill:Administratör:Småstads Kommun";
            ViewBag.ListOfDepartments = repository.Departments;

            var myEmployee = HttpContext.Session.GetJson<Employee>("NewEmployee");
            if (myEmployee == null)
            {
                ViewBag.FailMessage = "";
                return View();
            }
            else
            {
                ViewBag.FailMessage = message;
                HttpContext.Session.Remove("NewEmployee");
                return View(myEmployee);
            }
        }
        public ViewResult ValidateRemove(string id)
        {
            ViewBag.Title = "Tabort:Administratör:Småstads Kommun";

            var employeeDetail = repository.Employees.Where(te => te.EmployeeId == id).FirstOrDefault();

            return View(employeeDetail);
        }

        /*Saves a new user account and also a matching new Employee object. Account username will be the same as EmployeeId.
         If something goes wrong the view AddUser will be loaded again with a string message that contains the errormessage.*/
        public IActionResult SaveUser(Employee employee)
        {
            Employee newEmployee = new Employee
            {
                EmployeeId = employee.EmployeeId,
                EmployeePassword = employee.EmployeePassword,
                EmployeeName = employee.EmployeeName,
                RoleTitle = employee.RoleTitle,
                DepartmentId = employee.DepartmentId
            };

            if ((employee.RoleTitle == "Coordinator" && employee.DepartmentId != "D00") ||
                (employee.RoleTitle == "Administrator" && employee.DepartmentId != "D00"))
            {
                HttpContext.Session.SetJson("NewEmployee", newEmployee);
                return RedirectToAction("AddUser", 
                    new { message = "Coordinator och Administrator ska bara höra till Småstads Kommun." }); 
            }
            if ((employee.RoleTitle != "Coordinator" && employee.RoleTitle != "Administrator") &&
                (employee.DepartmentId == "D00"))
            {
                HttpContext.Session.SetJson("NewEmployee", newEmployee);
                return RedirectToAction("AddUser",
                    new { message = "Endast Coordinator och Administrator ska höra till Småstads Kommun." }); 
            }

            if (repository.SaveEmployee(newEmployee))
            {
                AccountManager.CreateUser(employee.EmployeeId, employee.EmployeePassword, employee.RoleTitle).Wait();
                return RedirectToAction("StartAdministrator");
            }
            else
            {
                HttpContext.Session.SetJson("NewEmployee", newEmployee);
                return RedirectToAction("AddUser",
                    new { message = "Ett konto med detta ID finns redan." });
            }
        }

        /*Updates a user account and its matching Employee object. Session "ResultMessage" will be used to send a confirmation
         message or a error message back to EditUser-view.*/
        public IActionResult UpdateUser(Employee employee)
        {
            //get the previous Employee from before updating, so that AccountManager can get the previous password and the previous role
            var previousData = HttpContext.Session.GetJson<Employee>("PreviousEmployeeData");
            HttpContext.Session.Remove("PreviousEmployeeData");

            Employee newEmployee = new Employee
            {
                EmployeeId = previousData.EmployeeId,
                EmployeePassword = employee.EmployeePassword,
                EmployeeName = employee.EmployeeName,
                RoleTitle = employee.RoleTitle,
                DepartmentId = employee.DepartmentId
            };

            if ((employee.RoleTitle == "Coordinator" && employee.DepartmentId != "D00") ||
                (employee.RoleTitle == "Administrator" && employee.DepartmentId != "D00"))
            {
                HttpContext.Session.SetJson("ResultMessage", "Coordinator och Administrator ska bara höra till Småstads Kommun.");
                return RedirectToAction("EditUser",
                    new { id = previousData.EmployeeId });
            }
            if ((employee.RoleTitle != "Coordinator" || employee.RoleTitle != "Administrator") &&
                (employee.DepartmentId == "D00"))
            {
                HttpContext.Session.SetJson("ResultMessage", "Endast Coordinator och Administrator ska höra till Småstads Kommun.");
                return RedirectToAction("EditUser",
                    new { id = previousData.EmployeeId });
            }

            repository.UpdateEmployee(newEmployee);
            AccountManager.UpdateUser(previousData.EmployeeId, previousData.EmployeePassword, 
                newEmployee.EmployeePassword, previousData.RoleTitle, newEmployee.RoleTitle).Wait();

            HttpContext.Session.SetJson("ResultMessage", "Sparat.");
            return RedirectToAction("EditUser", new { id = previousData.EmployeeId } );
        }

        /*Remove a specified account and its matching Employee object.*/
        public IActionResult RemoveUser(string id)
        {
            repository.RemoveEmployee(id);
            AccountManager.RemoveUser(id).Wait();
            return RedirectToAction("StartAdministrator");
        }

        /*Saves a List of Employees (to a session) that are filtered by either roletitle or department.*/
        public IActionResult FilterEmployees(string roletitle, string departmentid)
        {
            List<Employee> employees = new List<Employee>();
            if (roletitle == "all" && departmentid == "all")
                return RedirectToAction("StartAdministrator");
            else
            {
                if (roletitle != "all" && departmentid != "all")
                    foreach (Employee employee in repository.Employees)
                    {
                        if (employee.RoleTitle == roletitle && employee.DepartmentId == departmentid)
                            employees.Add(employee);
                    }
                else if (departmentid == "all")
                    foreach (Employee employee in repository.Employees)
                    {
                        if (employee.RoleTitle == roletitle)
                            employees.Add(employee);
                    }
                else if (roletitle == "all")
                    foreach (Employee employee in repository.Employees)
                    {
                        if (employee.DepartmentId == departmentid)
                            employees.Add(employee);
                    }
                HttpContext.Session.SetJson("FilteredEmployeeList", employees);
                return RedirectToAction("StartAdministrator");
            }
        }

        /*Saves a List of Employees of which the username/ID matches the specified employeeid.*/
        public IActionResult SearchEmployees(string employeeid)
        {
            List<Employee> employees = new List<Employee>();
            foreach (Employee employee in repository.Employees)
            {
                if (employee.EmployeeId == employeeid)
                    employees.Add(employee);
            }
            HttpContext.Session.SetJson("SearchedEmployeeList", employees);
            return RedirectToAction("StartAdministrator");
        }

        /*Returns the list of Employees to be displayed. Employees are retrieved from either session FilteredEmployeeList
        or SearchedEmployeeList, depending on which Action the user called. If neither exists it will be the same
        Employees as in repository.Employees.*/
        List<Employee> GetEmployeesToShow()
        {
            var employees = HttpContext.Session.GetJson<List<Employee>>("FilteredEmployeeList");
            if (employees != null)
                HttpContext.Session.Remove("FilteredEmployeeList");
            else
            {
                employees = HttpContext.Session.GetJson<List<Employee>>("SearchedEmployeeList");
                if (employees != null)
                    HttpContext.Session.Remove("SearchedEmployeeList");
                else
                {
                    employees = new List<Employee>();
                    foreach (Employee employee in repository.Employees) employees.Add(employee);
                }
            }
            return employees;
        }
    }
}

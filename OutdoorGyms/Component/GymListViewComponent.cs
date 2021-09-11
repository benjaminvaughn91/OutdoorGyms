using OutdoorGyms.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace OutdoorGyms.Component
{
    /*This viewcomponent will display the list of Gyms on the Start-pages for each role*/
    public class GymListViewComponent : ViewComponent
    {
        private IGymRepository repository;
        public GymListViewComponent(IGymRepository repo)
        {
            repository = repo;
        }
        public IViewComponentResult Invoke(List<Gym> gymList)
        {
            ViewBag.GymList = gymList;
            ViewBag.HasGyms = (gymList.Count > 0) ? true : false;

            ViewBag.UserRole = "Anyone";
            ViewBag.LoggedIn = false;
            var currentEmployee = repository.GetCurrentEmployee();
            if (currentEmployee != null)
            {
                ViewBag.UserRole = currentEmployee.RoleTitle;
                ViewBag.LoggedIn = true;
            }

            return View("GymList", repository);
        }
    }
}

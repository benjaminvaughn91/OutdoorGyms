using EnvironmentCrime.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnvironmentCrime.Component
{
    /*This viewcomponent will display the list of Errands on the Start-pages for each role*/
    public class ErrandListViewComponent : ViewComponent
    {
        private ICrimeRepository repository;
        public ErrandListViewComponent(ICrimeRepository repo)
        {
            repository = repo;
        }
        public IViewComponentResult Invoke(List<Errand> errandList)
        {
            ViewBag.ErrandList = errandList;
            ViewBag.HasErrands = (errandList.Count > 0) ? true : false;

            //ViewBag.UserRole will be used for asp-controller and asp-action to find the right page to go to.
            var currentEmployee = repository.GetCurrentEmployee();
            ViewBag.UserRole = currentEmployee.RoleTitle;

            return View("ErrandList", repository);
        }
    }
}

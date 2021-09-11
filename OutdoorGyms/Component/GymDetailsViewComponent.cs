using OutdoorGyms.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace OutdoorGyms.Component
{
    /*This viewcomponent will display the details of the selected Gym in the Gym-pages for each role.*/
    public class GymDetailsViewComponent : ViewComponent
    {
        private IGymRepository repository;
        public GymDetailsViewComponent(IGymRepository repo)
        {
            repository = repo;
        }
        public IViewComponentResult Invoke(string id) 
        {
            var gymDetail = repository.GetGymDetail(int.Parse(id));

            /*County, status and employee-names are connected to the Gym by Id, so their names 
             * will be retrieved with methods in the repository.*/
            ViewBag.CountyName = repository.GetGymCounty(gymDetail);
            ViewBag.StatusName = repository.GetGymStatus(gymDetail);

            ViewBag.LoggedIn = false;
            var currentEmployee = repository.GetCurrentEmployee();
            if (currentEmployee != null)
                if (currentEmployee.RoleTitle == "Moderator" || currentEmployee.RoleTitle == "Contributor")
                    ViewBag.LoggedIn = true;

            ViewBag.Description = gymDetail.Description;

            if (gymDetail.Pictures.Count() > 0)
            {
                ViewBag.ShowPictures = true;
                ViewBag.Pictures = gymDetail.Pictures;
            }
            else ViewBag.ShowPictures = false;

            return View("GymDetails", gymDetail);
        }
    }
}

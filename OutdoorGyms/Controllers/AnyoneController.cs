using OutdoorGyms.Models;
using Microsoft.AspNetCore.Mvc;

namespace OutdoorGyms.Controllers
{
    /*Controller class with an Action for each View in the Anyone-folder*/
    public class AnyoneController : Controller
    {
        private IGymRepository repository;
        public AnyoneController(IGymRepository repo)
        {
            repository = repo;
        }
        public ViewResult Contact()
        {
            ViewBag.Title = "Contact:OutdoorGyms";
            return View();
        }
        public ViewResult About()
        {
            ViewBag.Title = "About:OutdoorGyms";
            return View();
        }
        public ViewResult GymAnyone(string id)
        {
            ViewBag.Title = "Gym:LoggedOut:OutdoorGyms";
            ViewBag.ID = id;

            return View();
        }
    }
}

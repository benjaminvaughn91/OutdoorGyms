using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnvironmentCrime.Models;
using Microsoft.AspNetCore.Mvc;
using SessionTest.Infrastructure;

namespace EnvironmentCrime.Controllers
{
    /*Controller class with an Action for each View in the Citizen-folder*/
    public class CitizenController : Controller
    {
        private ICrimeRepository repository;
        public CitizenController(ICrimeRepository repo)
        {
            repository = repo;
        }
        public ViewResult Contact()
        {
            ViewBag.Title = "Kontakt:Småstads Kommun";
            return View();
        }
        public ViewResult Faq()
        {
            ViewBag.Title = "FAQ:Småstads Kommun";
            return View();
        }
        public ViewResult Services()
        {
            ViewBag.Title = "Tjänster:Småstads Kommun";
            return View();
        }
        public ViewResult Thanks()
        {
            ViewBag.Title = "Tack:Småstads Kommun";
            Errand err = HttpContext.Session.GetJson<Errand>("NewErrand");
            repository.SaveErrand(err);
            ViewBag.RefNumber = err.RefNumber;
            HttpContext.Session.Remove("NewErrand"); 
            return View();
        }
        public ViewResult Validate(Errand errand)
        {
            ViewBag.Title = "Bekräftelse:Småstads Kommun";
            ViewBag.Place = errand.Place;
            ViewBag.TypeOfCrime = errand.TypeOfCrime;
            ViewBag.DateOfObservation = errand.DateOfObservation;
            ViewBag.InformerName = errand.InformerName;
            ViewBag.InformerPhone = errand.InformerPhone;
            ViewBag.Observation = errand.Observation;

            HttpContext.Session.SetJson("NewErrand", errand);
            return View(errand);
        }
    }
}

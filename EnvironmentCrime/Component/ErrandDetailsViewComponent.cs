using EnvironmentCrime.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnvironmentCrime.Component
{
    /*This viewcomponent will display the details of the selected Errand in the Crime-pages for each role.*/
    public class ErrandDetailsViewComponent : ViewComponent
    {
        private ICrimeRepository repository;
        public ErrandDetailsViewComponent(ICrimeRepository repo)
        {
            repository = repo;
        }
        public IViewComponentResult Invoke(string id) 
        {
            var errandDetail = repository.GetErrandDetail(int.Parse(id));

            /*Department, status and employee-names are connected to the Errand by Id, so their names 
             * will be retrieved with methods in the repository.*/
            ViewBag.DepartmentName = repository.GetErrandDepartment(errandDetail);
            ViewBag.StatusName = repository.GetErrandStatus(errandDetail);
            ViewBag.EmployeeName = repository.GetErrandEmployee(errandDetail);

            if (errandDetail.Samples.Count() > 0)
            {
                ViewBag.ShowSamples = true;
                ViewBag.Samples = errandDetail.Samples;
            }
            else ViewBag.ShowSamples = false;

            if (errandDetail.Pictures.Count() > 0)
            {
                ViewBag.ShowPictures = true;
                ViewBag.Pictures = errandDetail.Pictures;
            }
            else ViewBag.ShowPictures = false;

            return View("ErrandDetails", errandDetail);
        }
    }
}

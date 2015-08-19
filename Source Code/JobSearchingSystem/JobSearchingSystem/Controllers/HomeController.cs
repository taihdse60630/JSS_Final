using JobSearchingSystem.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using JobSearchingSystem.Models;

namespace JobSearchingSystem.Controllers
{
    public class HomeController : Controller
    {
        private HomeUnitOfWork homeUnitOfWork = new HomeUnitOfWork();

        public ActionResult TestJquery()
        {
            return View();
        }
        public ActionResult Index()
        {
            ViewBag.message = TempData["message"];

            HIndexViewModel hIndexViewModel = new HIndexViewModel();
            homeUnitOfWork.getAllJob(hIndexViewModel);
            hIndexViewModel.jobCities = homeUnitOfWork.getAllCities();
            hIndexViewModel.jobCategories = homeUnitOfWork.getAllCategories();
            hIndexViewModel.schoolLevelList = homeUnitOfWork.getAllSchoolLevel();
            hIndexViewModel.purchaseAdvertiseTypeA = homeUnitOfWork.getPurchaseAdvertise("A");
            hIndexViewModel.purchaseAdvertiseTypeB = homeUnitOfWork.getPurchaseAdvertise("B");
            hIndexViewModel.purchaseAdvertiseTypeC = homeUnitOfWork.getPurchaseAdvertise("C");

            var a = homeUnitOfWork.getPurchaseAdvertise("A").ToArray() ;
            var b = homeUnitOfWork.getPurchaseAdvertise("B").ToArray();
            var c = homeUnitOfWork.getPurchaseAdvertise("C").ToArray();

            return View(hIndexViewModel);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Find(HIndexViewModel model)
        {
            JFindViewModel jFindViewModel = new JFindViewModel();
            jFindViewModel.searchString = model.searchString;        
            jFindViewModel.minSalary = model.minSalary;           
            jFindViewModel.schoolLevel = model.schoolLevel;
            TempData["searchJobCities"] = model.searchJobCities;
            TempData["searchJobCategories"] = model.searchJobCategories;
            return RedirectToAction("Find", "Job", jFindViewModel);
        }
    }
}
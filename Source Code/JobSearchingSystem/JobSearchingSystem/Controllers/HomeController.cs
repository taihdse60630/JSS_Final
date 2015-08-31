using JobSearchingSystem.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using JobSearchingSystem.Models;

namespace JobSearchingSystem.Controllers
{
    [MessageFilter]
    public class HomeController : Controller
    {
        private HomeUnitOfWork homeUnitOfWork = new HomeUnitOfWork();

        public ActionResult TestJquery()
        {
            return View();
        }
        public ActionResult Index()
        {
            HIndexViewModel hIndexViewModel = new HIndexViewModel();
            hIndexViewModel.jobList = homeUnitOfWork.getAllJob();
            hIndexViewModel.jobCities = homeUnitOfWork.getAllCities();
            hIndexViewModel.jobCategories = homeUnitOfWork.getAllCategories();
            hIndexViewModel.schoolLevelList = homeUnitOfWork.getAllSchoolLevel();
            hIndexViewModel.purchaseAdvertiseTypeA = homeUnitOfWork.getPurchaseAdvertise("A");
            hIndexViewModel.purchaseAdvertiseTypeB = homeUnitOfWork.getPurchaseAdvertise("B");
            hIndexViewModel.purchaseAdvertiseTypeC = homeUnitOfWork.getPurchaseAdvertise("C");
            hIndexViewModel.topicList = homeUnitOfWork.GetAllTopic();
            //var a = homeUnitOfWork.getPurchaseAdvertise("A").ToArray() ;
            //var b = homeUnitOfWork.getPurchaseAdvertise("B").ToArray();
            //var c = homeUnitOfWork.getPurchaseAdvertise("C").ToArray();

            return View(hIndexViewModel);
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

        public ActionResult SortJob(string type)
        {
            
            HIndexViewModel model = new HIndexViewModel();
            model.jobList = homeUnitOfWork.getAllJobAreSorted(type);
            int length = model.jobList.Count();
            return PartialView("JobPartial", model);
        }
    }
}
using JobSearchingSystem.DAL;
using JobSearchingSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JobSearchingSystem.Controllers
{
    [MessageFilter]
    public class JobPackageController : Controller
    {
        private PackageUnitOfWork packageUnitOfWork = new PackageUnitOfWork();

        //
        // GET: /JobPackage/
        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Recruiter")]
        public ActionResult Choose()
        {
            JobPackageViewModels model = new JobPackageViewModels();
            IEnumerable<JobPackage> jobPackageList = packageUnitOfWork.GetAllPackage();
            model.jobPackageList = jobPackageList;
            return View(model);
        }

        [Authorize(Roles = "Recruiter")]
        public ActionResult Invoice(string jobPackageID)
        {
            if (String.IsNullOrEmpty(jobPackageID))
            {
                return RedirectToAction("Choose");
            }
            else
            {
                try{
                    InvoiceVIewModels model = new InvoiceVIewModels();
                    model.jobPackage = packageUnitOfWork.GetJobPackageById(Int32.Parse(jobPackageID));
                    return View(model);
                }catch(Exception e){
                    return RedirectToAction("Choose");
                }
             
            }
            
        }

         //[Authorize(Roles = "Recruiter")]
        public JsonResult SendPackageRequest(int packageQuantity, int jobPackageID)
        {
            int quantity = packageQuantity;
            int packageID =jobPackageID;
            string userID = packageUnitOfWork.AspNetUserRepository.Get(s => s.UserName == User.Identity.Name).FirstOrDefault().Id; ;
            bool result = packageUnitOfWork.CreateJobPackageRequest(userID, packageID, quantity);

            return Json(result, JsonRequestBehavior.AllowGet);      
  
        }

        [Authorize(Roles = "Staff")]
        public ActionResult JobPackageRequestList()
        {
           IEnumerable<JPurchaseJobPackage> purchaseJobPackageList = packageUnitOfWork.GetAllJobPackageRequest();
           JobPackageRequestViewModels model = new JobPackageRequestViewModels();
           model.purchaseJobPackageList = purchaseJobPackageList;
           return View(model);
        }

          [Authorize(Roles = "Staff")]
        public ActionResult AcceptJobPackageRequest(int purchaseJobPackageID)
        {
            string userID = packageUnitOfWork.AspNetUserRepository.Get(s => s.UserName == User.Identity.Name).FirstOrDefault().Id;
            packageUnitOfWork.AcceptJobPackageRequest(purchaseJobPackageID, userID);
            return RedirectToAction("JobPackageRequestList");
        }

          [Authorize(Roles = "Staff")]
        public ActionResult DeleteJobPackageRequest(int purchaseJobPackageID)
        {
            string userID = packageUnitOfWork.AspNetUserRepository.Get(s => s.UserName == User.Identity.Name).FirstOrDefault().Id;
            packageUnitOfWork.DeleteJobPackageRequest(purchaseJobPackageID, userID);
            return RedirectToAction("JobPackageRequestList");
        }

          [Authorize(Roles = "Staff")]
          public ActionResult AcceptMultiJobPackage(string purchaseJobPackageIDList)
           {
            try
            {
                List<int> list = purchaseJobPackageIDList.Split(',').Select(int.Parse).ToList();
                string userID = packageUnitOfWork.AspNetUserRepository.Get(s => s.UserName == User.Identity.Name).FirstOrDefault().Id;
                List<int> listAccept = new List<int>();
                foreach (var item in list)
                {
                    listAccept.Add(item);
                }
                packageUnitOfWork.AccepMultitJobPackageRequest(listAccept, userID);
            }
            catch (Exception e)
            {
                return RedirectToAction("JobPackageRequestList");
            }

            return RedirectToAction("JobPackageRequestList");
        }


          [Authorize(Roles = "Staff")]
          public ActionResult RejectMultiJobPackage(string purchaseJobPackageIDList)
          {
              try
              {
                  List<int> list = purchaseJobPackageIDList.Split(',').Select(int.Parse).ToList();
                  string userID = packageUnitOfWork.AspNetUserRepository.Get(s => s.UserName == User.Identity.Name).FirstOrDefault().Id;
                  List<int> listAccept = new List<int>();
                  foreach (var item in list)
                  {
                      listAccept.Add(item);
                  }
                  packageUnitOfWork.RejectMultiJobPackage(listAccept, userID);
              }
              catch (Exception e)
              {
                  return RedirectToAction("JobPackageRequestList");
              }

              return RedirectToAction("JobPackageRequestList");
          }


          [Authorize(Roles = "Staff")]
        public ActionResult DeleteMultiJobPackage(string[] purchaseJobPackageID)
        {
            try
            {
                string userID = packageUnitOfWork.AspNetUserRepository.Get(s => s.UserName == User.Identity.Name).FirstOrDefault().Id;
                List<int> listDelete = new List<int>();
                foreach (var item in purchaseJobPackageID)
                {
                    listDelete.Add(Int32.Parse(item));
                }
                packageUnitOfWork.DeleteMultitJobPackageRequest(listDelete, userID);
            }
            catch (Exception e)
            {
                return RedirectToAction("JobPackageRequestList");
            }

            return RedirectToAction("JobPackageRequestList");
        }

          [Authorize(Roles = "Recruiter")]
          public ActionResult JobPackageRequestListRecruiter()
          {
              string userID = packageUnitOfWork.AspNetUserRepository.Get(s => s.UserName == User.Identity.Name).FirstOrDefault().Id;
              IEnumerable<JPurchaseJobPackage> purchaseJobpackage = packageUnitOfWork.GetAllJobPackageRequest(userID);
              JobPackageRequestViewModels model = new JobPackageRequestViewModels();
              model.purchaseJobPackageList = purchaseJobpackage;
              return View(model);
          }

	}
}
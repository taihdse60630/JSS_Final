using JobSearchingSystem.DAL;
using JobSearchingSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JobSearchingSystem.Controllers
{
    public class ReportController : Controller
    {
        private ReportUnitOfWork reportUnitOfWork = new ReportUnitOfWork();
        //
        // GET: /Report/
        [Authorize(Roles = "Staff,Admin,Manager")]
        public ActionResult Index()
        {
            ReportViewModels model = new ReportViewModels();
            model.reportList = reportUnitOfWork.GetAllReport();

            return View(model);
        }

        [Authorize(Roles = "Staff,Admin,Manager")]
        public ActionResult DeleteReport(int reportID)
        {
            reportUnitOfWork.DeleteReport(reportID);

            return RedirectToAction("Index");
        }

        [Authorize]
        public JsonResult SendReport(string reportContent, string refrenceLink)
        {
            string senderId = reportUnitOfWork.AspNetUserRepository.Get(s => s.UserName == User.Identity.Name).FirstOrDefault().Id;
            bool result = reportUnitOfWork.createReport(reportContent, senderId, refrenceLink);
           
            return Json(result, JsonRequestBehavior.AllowGet);      
        }
	}
}
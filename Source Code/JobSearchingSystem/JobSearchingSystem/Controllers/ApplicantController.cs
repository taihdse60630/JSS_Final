using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using JobSearchingSystem.DAL;

namespace JobSearchingSystem.Controllers
{
    [Authorize(Roles = "Recruiter")]
    public class ApplicantController : Controller
    {
        private ApplicantUnitOfWork applicantUnitOfWork = new ApplicantUnitOfWork();

        //List all applicants for a specific job
        public ActionResult List(int id)
        {
            ViewBag.JobID = id;
            return View(applicantUnitOfWork.GetApplicantByJobID(id));
        }

        //Approve Applicant
        public ActionResult Approve(string applicantID, int jobID)
        {            
            if (applicantUnitOfWork.ApproveApplicant(applicantID, jobID))
            {
                return RedirectToAction("List", new { id = jobID });
            }
            return RedirectToAction("List", new { id = jobID });
        }

        //Reject applicant
        public ActionResult Disapprove(string applicantID, int jobID)
        {
            if (applicantUnitOfWork.RejectApplicant(applicantID, jobID))
            {
                return RedirectToAction("List", new { id = jobID });
            }
            return RedirectToAction("List", new { id = jobID });
        }
	}
}
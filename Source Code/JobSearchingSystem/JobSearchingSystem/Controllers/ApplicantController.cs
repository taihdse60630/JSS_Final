using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using JobSearchingSystem.DAL;

namespace JobSearchingSystem.Controllers
{
    [Authorize(Roles = "Recruiter")]
    [MessageFilter]
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
        [HttpPost]
        public ActionResult Approve(string applicantID, int jobID)
        {            
            if (applicantUnitOfWork.ApproveApplicant(applicantID, jobID))
            {
                string username = applicantUnitOfWork.AspNetUserRepository.GetByID(applicantID).UserName;
                string jobTitle = applicantUnitOfWork.JobRepository.GetByID(jobID).JobTitle;
                applicantUnitOfWork.SendEmail(username, "Thông báo kết quả", "Chào bạn,<br>Đơn xin ứng tuyển vào công việc " + jobTitle + " của bạn đã được duyệt.<br>Best Regards,<br>JSS");
                TempData["successmessage"] = "Duyệt thành công.";
            }
            else
            {
                TempData["errormessage"] = "Duyệt thất bại!";
            }

            return RedirectToAction("List", new { id = jobID });
        }

        //Reject applicant
        [HttpPost]
        public ActionResult Disapprove(string applicantID, int jobID)
        {
            if (applicantUnitOfWork.RejectApplicant(applicantID, jobID))
            {
                string username = applicantUnitOfWork.AspNetUserRepository.GetByID(applicantID).UserName;
                string jobTitle = applicantUnitOfWork.JobRepository.GetByID(jobID).JobTitle;
                applicantUnitOfWork.SendEmail(username, "Thông báo kết quả", "Chào bạn,<br>Đơn xin ứng tuyển vào công việc " + jobTitle + " của bạn đã bị từ chối.<br>Best Regards,<br>JSS");
                TempData["successmessage"] = "Từ chối thành công.";
            }
            else
            {
                TempData["errormessage"] = "Từ chối thất bại!";
            }

            return RedirectToAction("List", new { id = jobID });
        }

        [HttpPost]
        public ActionResult SendMessage(string receiverUsername, string messageContent, int jobID, string subject)
        {
            if (String.IsNullOrEmpty(receiverUsername) || String.IsNullOrEmpty(messageContent))
            {
                TempData["errormessage"] = "Dữ liệu không hợp lệ!";
                return RedirectToAction("List", new { id = jobID });
            }
            //ArrayList list = new ArrayList();
            //list.AddRange(receiver.Split(new char[] { ',' }));
            //for (int i = 0; i < list.Count; i++)
            //{
            //    for (int j = i + 1; j < list.Count; j++)
            //    {
            //        if (list[i].ToString() == list[j].ToString())
            //        {
            //            list.Remove(list[j]);
            //        }
            //    }
            //}

            AspNetUser user = applicantUnitOfWork.AspNetUserRepository.Get(s => s.UserName == receiverUsername).FirstOrDefault();
            if (user == null)
            {
                TempData["errormessage"] = "Không tìm thấy thông tin tài khoản!";
                return RedirectToAction("List", new { id = jobID });
            }

            AppliedJob appliedJob = applicantUnitOfWork.AppliedJobRepository.Get(s => s.JobSeekerID == user.Id && s.JobID == jobID).FirstOrDefault();
            if (appliedJob == null)
            {
                TempData["errormessage"] = "Không tìm thấy thông tin đơn!";
                return RedirectToAction("List", new { id = jobID });
            }

            appliedJob.Status = 1;
            applicantUnitOfWork.AppliedJobRepository.Update(appliedJob);
            applicantUnitOfWork.Save();

            //applicantUnitOfWork.SendMessage(User.Identity.Name, list, messageContent);
            messageContent = HttpUtility.UrlDecode(messageContent);
            if (String.IsNullOrEmpty(subject))
            {
                applicantUnitOfWork.SendEmail(receiverUsername, "Thông báo phỏng vấn", messageContent);
                TempData["successmessage"] = "Mail thông báo phỏng vấn đã được gửi đi.";
            }
            else
            {
                applicantUnitOfWork.SendEmail(receiverUsername, subject, messageContent);
                TempData["successmessage"] = "Mail đã được gửi đi.";
            }

            return RedirectToAction("List", new { id = jobID });
        }
	}
}
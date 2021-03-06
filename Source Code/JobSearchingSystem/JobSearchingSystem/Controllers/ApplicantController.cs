﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using JobSearchingSystem.DAL;
using JobSearchingSystem.Models;

namespace JobSearchingSystem.Controllers
{
    [Authorize(Roles = "Recruiter")]
    [MessageFilter]
    public class ApplicantController : Controller
    {
        private ApplicantUnitOfWork applicantUnitOfWork = new ApplicantUnitOfWork();

        //List all applicants for a specific job
        public ActionResult List(int? id)
        {
            int jobId = id.GetValueOrDefault();
            if (jobId > 0)
            {
                Job oldJob = applicantUnitOfWork.JobRepository.GetByID(jobId);
                if (oldJob != null)
                {
                    AspNetUser recruiter = applicantUnitOfWork.AspNetUserRepository.Get(s => s.UserName == User.Identity.Name).FirstOrDefault();

                    if (oldJob.RecruiterID != recruiter.Id)
                    {
                        TempData["errormessage"] = "Bạn không có quyền xem ứng viên của công việc này!";
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ViewBag.JobID = jobId;
                        return View(applicantUnitOfWork.GetApplicantByJobID(jobId));
                    }
                }
                else
                {
                    TempData["errormessage"] = "Không tìm thấy công việc!";
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                TempData["errormessage"] = "Lỗi dữ liệu!";
                return RedirectToAction("Index", "Home");
            }
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
            MessageController messageController = new MessageController();

            if (String.IsNullOrEmpty(receiverUsername) || String.IsNullOrEmpty(messageContent))
            {
                TempData["errormessage"] = "Dữ liệu không hợp lệ!";
                return RedirectToAction("List", new { id = jobID });
            }

            AspNetUser user = applicantUnitOfWork.AspNetUserRepository.Get(s => s.UserName == receiverUsername).FirstOrDefault();
            if (user == null)
            {
                TempData["errormessage"] = "Không tìm thấy thông tin tài khoản!";
                return RedirectToAction("List", new { id = jobID });
            }
             //ThienNN
            string messageForMail = "Chào bạn,<br><br>Bạn vừa nhận được tin nhắn từ nhà tuyển dụng vui lòng đăng nhập vào hệ thống chúng tôi bằng link sau để kiểm tra hộp tin nhắn <br /> http://localhost:64977/Message/List <br><br>Best Regards,<br>JSS";

            if (String.IsNullOrEmpty(subject))
            {
                applicantUnitOfWork.SendEmail(receiverUsername, "Thông báo tin nhắn mới", messageForMail);
                messageController.SendMessageInterview(User.Identity.Name, receiverUsername, messageContent);
                TempData["successmessage"] = "Tin nhắn của bạn đã được gửi đi.";
            }
            else
            {
                applicantUnitOfWork.SendEmail(receiverUsername, subject, messageContent);
                TempData["successmessage"] = "Mail đã được gửi đi.";
            }

            return RedirectToAction("List", new { id = jobID });
        }

        public JsonResult GetMatchingDetail(string applicantID, int jobID)
        {
            if (!String.IsNullOrEmpty(applicantID))
            {
                AppliedJob appliedJob = applicantUnitOfWork.AppliedJobRepository.Get(s => s.JobSeekerID == applicantID && s.JobID == jobID).FirstOrDefault();

                if (appliedJob != null)
                {
                    int profileID = appliedJob.ProfileID;

                    var sosanhItemList = applicantUnitOfWork.GetMatchingDetail(profileID, jobID);

                    return Json(sosanhItemList, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return this.Json(string.Empty);
                }
            }
            else
            {
                return this.Json(string.Empty);
            }
        }

        public JsonResult GetMatchingDetail2(string applicantID1, string applicantID2, int jobID)
        {
            if (!String.IsNullOrEmpty(applicantID1)
             && !String.IsNullOrEmpty(applicantID2))
            {
                AppliedJob appliedJob1 = applicantUnitOfWork.AppliedJobRepository.Get(s => s.JobSeekerID == applicantID1 && s.JobID == jobID).FirstOrDefault();
                AppliedJob appliedJob2 = applicantUnitOfWork.AppliedJobRepository.Get(s => s.JobSeekerID == applicantID2 && s.JobID == jobID).FirstOrDefault();

                if (appliedJob1 != null && appliedJob2 != null)
                {
                    int profileID1 = appliedJob1.ProfileID;
                    int profileID2 = appliedJob2.ProfileID;

                    var sosanhItemList1 = applicantUnitOfWork.GetMatchingDetail(profileID1, jobID);
                    var sosanhItemList2 = applicantUnitOfWork.GetMatchingDetail(profileID2, jobID);

                    Jobseeker applicant1 = applicantUnitOfWork.JobseekerRepository.GetByID(applicantID1);
                    Jobseeker applicant2 = applicantUnitOfWork.JobseekerRepository.GetByID(applicantID2);

                    return Json(new { profile1 = sosanhItemList1, profile2 = sosanhItemList2, applicantname1 = applicant1.FullName, applicantname2 = applicant2.FullName }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return this.Json(string.Empty);
                }
            }
            else
            {
                return this.Json(string.Empty);
            }
        }

        public JsonResult GetMatchingDetail3(string profileID, int jobID)
        {
            if (!String.IsNullOrEmpty(profileID))
            {
                int profileIDNum = Int32.Parse(profileID);

                var sosanhItemList = applicantUnitOfWork.GetMatchingDetail(profileIDNum, jobID);

                return Json(sosanhItemList, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return this.Json(string.Empty);
            }
        }

        public JsonResult GetMatchingDetail4(string profileID1, string profileID2, int jobID)
        {
            if (!String.IsNullOrEmpty(profileID1)
             && !String.IsNullOrEmpty(profileID2))
            {
                int profileID1Num = Int32.Parse(profileID1);
                int profileID2Num = Int32.Parse(profileID2);

                var sosanhItemList1 = applicantUnitOfWork.GetMatchingDetail(profileID1Num, jobID);
                var sosanhItemList2 = applicantUnitOfWork.GetMatchingDetail(profileID2Num, jobID);

                Jobseeker applicant1 = applicantUnitOfWork.JobseekerRepository.GetByID(applicantUnitOfWork.ProfileRepository.GetByID(profileID1Num).JobSeekerID);
                Jobseeker applicant2 = applicantUnitOfWork.JobseekerRepository.GetByID(applicantUnitOfWork.ProfileRepository.GetByID(profileID2Num).JobSeekerID);

                return Json(new { profile1 = sosanhItemList1, profile2 = sosanhItemList2, applicantname1 = applicant1.FullName, applicantname2 = applicant2.FullName }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return this.Json(string.Empty);
            }
        }
	}
}
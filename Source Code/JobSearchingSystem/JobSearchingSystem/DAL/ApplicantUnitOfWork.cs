using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using JobSearchingSystem.Models;
using System.Net.Mail;

namespace JobSearchingSystem.DAL
{
    public class ApplicantUnitOfWork : UnitOfWork
    {
        //Get all applicants by job ID
        public IEnumerable<ApplicantItem> GetApplicantByJobID(int id)
        {
            List<ApplicantItem> listApplicant = new List<ApplicantItem>();
            foreach (AppliedJob item in this.AppliedJobRepository.Get(filter: applicant => applicant.JobID == id && applicant.IsDeleted == false))
            {
                AspNetUser user = this.AspNetUserRepository.Get(filter: m => m.Id == item.JobSeekerID).FirstOrDefault();
                string userId = user.Id;
                string username = user.UserName;
                Contact contact = this.ContactRepository.GetByID(userId);
                string fullname = "";
                if (contact != null)
                {
                    fullname = contact.FullName;
                }
                else
                {
                    fullname = username;
                }
                Profile profile = this.ProfileRepository.Get(s => s.JobSeekerID == userId && s.IsActive == true && s.IsDeleted == false).LastOrDefault();
                listApplicant.Add(new ApplicantItem(item.JobSeekerID, item.Profile.Name, item.Profile.ExpectedPosition, item.ApplyDate, item.MatchingPercent, item.Status, username, fullname, profile != null ? profile.ProfileID : 0));
            }
            return listApplicant;
        }

        //Approve applicant
        public bool ApproveApplicant(string id, int jobID)
        {
            try
            {
                AppliedJob appliedJob = this.AppliedJobRepository.Get(applicant => applicant.JobSeekerID == id && applicant.JobID == jobID).SingleOrDefault();
                appliedJob.Status = 2;
                this.AppliedJobRepository.Update(appliedJob);
                this.Save();
                return true;
            }
            catch
            {
                return false;
            }
        }

        //Reject applicant
        public bool RejectApplicant(string id, int jobID)
        {
            try
            {
                AppliedJob appliedJob = this.AppliedJobRepository.Get(applicant => applicant.JobSeekerID == id && applicant.JobID == jobID).SingleOrDefault();
                appliedJob.Status = 3;
                this.AppliedJobRepository.Update(appliedJob);
                this.Save();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void SendEmail(string receiverUsername, string title, string content)
        {
            if (!String.IsNullOrEmpty(receiverUsername)
                && !String.IsNullOrEmpty(title)
                && !String.IsNullOrEmpty(content))
            {
                AspNetUser user = this.AspNetUserRepository.Get(s => s.UserName == receiverUsername).FirstOrDefault();
                if (user == null)
                {
                    return;
                }

                try
                {
                    MailMessage mail = new MailMessage();
                    SmtpClient smtpClient = new SmtpClient("smtp.gmail.com");

                    UnitOfWork unitOfWork = new UnitOfWork();
                    mail.From = new MailAddress("jss.noreply.email@gmail.com");
                    Jobseeker jobseeker = unitOfWork.JobseekerRepository.GetByID(user.Id);
                    Recruiter recruiter = unitOfWork.RecruiterRepository.GetByID(user.Id);
                    if (jobseeker != null)
                    {
                        mail.To.Add(jobseeker.Email);
                    }
                    else if (recruiter != null)
                    {
                        mail.To.Add(recruiter.Email);
                    }
                    else
                    {
                        return;
                    }
                    mail.Subject = "[JSS] " + title;
                    mail.IsBodyHtml = true;
                    mail.Body = content;

                    smtpClient.Port = 587;
                    smtpClient.Credentials = new System.Net.NetworkCredential("jss.noreply.email@gmail.com", "Kogarashi789");
                    smtpClient.EnableSsl = true;

                    smtpClient.Send(mail);
                }
                catch (Exception)
                {
                    return;
                }
            }
        }
    }
}
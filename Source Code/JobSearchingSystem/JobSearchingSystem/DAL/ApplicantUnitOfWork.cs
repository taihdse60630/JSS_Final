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
                appliedJob.Status = 1;
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

        public List<SosanhItem> GetMatchingDetail(int profileId, int jobId)
        {
            List<SosanhItem> matchingDetail = new List<SosanhItem>();

            Profile profile = this.ProfileRepository.GetByID(profileId);
            Job job = this.JobRepository.GetByID(jobId);

            if (profile != null && job != null)
            {
                // MinSalary - MaxSalary Nullable - 20
                decimal expectedSalary = profile.ExpectedSalary;
                decimal? minSalary = job.MinSalary;
                decimal? maxSalary = job.MaxSalary;
                SosanhItem sosanh1 = new SosanhItem();
                sosanh1.columnName = "Mức lương";
                if (minSalary == null && maxSalary == null)
                {
                    sosanh1.jobInfo = "Thỏa thuận";
                }
                else
                {
                    sosanh1.jobInfo = minSalary.ToString() + " => " + maxSalary.ToString();
                }
                if (expectedSalary == 0)
                {
                    sosanh1.applicantInfo = "Thỏa thuận";
                }
                else
                {
                    sosanh1.applicantInfo = expectedSalary.ToString();
                }
                if (expectedSalary == 0
                    || (minSalary != null && maxSalary != null && minSalary <= expectedSalary && expectedSalary <= maxSalary)
                    || (minSalary != null && maxSalary == null && minSalary <= expectedSalary)
                    || (minSalary == null && maxSalary != null && expectedSalary <= maxSalary))
                {
                    sosanh1.isSatisfied = true;
                }
                else
                {
                    sosanh1.isSatisfied = false;
                }
                matchingDetail.Add(sosanh1);

                // JobLevel_ID - 10
                JobLevel expectedJobLevel = this.JobLevelRepository.GetByID(profile.ExpectedJobLevel_ID);
                JobLevel jobLevel = this.JobLevelRepository.GetByID(job.JobLevel_ID);
                SosanhItem sosanh2 = new SosanhItem();
                sosanh2.columnName = "Vị trí";
                sosanh2.jobInfo = jobLevel.Name;
                sosanh2.applicantInfo = expectedJobLevel.Name;
                if (expectedJobLevel != null && jobLevel != null)
                {
                    if (jobLevel.LevelNum >= expectedJobLevel.LevelNum)
                    {
                        sosanh2.isSatisfied = true;
                    }
                    else
                    {
                        sosanh2.isSatisfied = false;
                    }
                }
                matchingDetail.Add(sosanh2);

                // MinSchoolLevel_ID - 10
                SchoolLevel highestSchoolLevel = this.SchoolLevelRepository.GetByID(profile.HighestSchoolLevel_ID);
                SchoolLevel minSchoolLevel = this.SchoolLevelRepository.GetByID(job.MinSchoolLevel_ID);
                SosanhItem sosanh3 = new SosanhItem();
                sosanh3.columnName = "Trình độ";
                sosanh3.jobInfo = minSchoolLevel.Name;
                sosanh3.applicantInfo = highestSchoolLevel.Name;
                if (highestSchoolLevel != null && minSchoolLevel != null)
                {
                    if (highestSchoolLevel.LevelNum >= minSchoolLevel.LevelNum)
                    {
                        sosanh3.isSatisfied = true;
                    }
                    else
                    {
                        sosanh3.isSatisfied = false;
                    }
                }
                matchingDetail.Add(sosanh3);

                // Skill (nhieu TH) - 20
                IEnumerable<int> jobSkillIdList = this.JobSkillRepository.Get(s => s.JobID == jobId && s.IsDeleted == false).Select(s => s.Skill_ID).AsEnumerable();
                IEnumerable<int> ownSkillIdList = this.OwnSkillRepository.Get(s => s.JobSeekerID == profile.JobSeekerID && s.IsDeleted == false).Select(s => s.Skill_ID).AsEnumerable();
                IEnumerable<int> skillIdIntersectList = jobSkillIdList.Intersect(ownSkillIdList);
                SosanhItem sosanh4 = new SosanhItem();
                sosanh4.columnName = "Kỹ năng";
                if (jobSkillIdList.Count() == 0)
                {
                    sosanh4.jobInfo = "Không yêu cầu";
                }
                else
                {
                    sosanh4.jobInfo = this.SkillRepository.GetByID(jobSkillIdList.ElementAt(0)).SkillTag;
                    for (int i = 1; i < jobSkillIdList.Count(); i++)
                    {
                        sosanh4.jobInfo += "; " + this.SkillRepository.GetByID(jobSkillIdList.ElementAt(i)).SkillTag;
                    }
                }
                if (ownSkillIdList.Count() == 0)
                {
                    sosanh4.applicantInfo = "Không có";
                }
                else
                {
                    sosanh4.applicantInfo = this.SkillRepository.GetByID(ownSkillIdList.ElementAt(0)).SkillTag;
                    for (int i = 1; i < ownSkillIdList.Count(); i++)
                    {
                        sosanh4.applicantInfo += "; " + this.SkillRepository.GetByID(ownSkillIdList.ElementAt(i)).SkillTag;
                    }
                }
                if (jobSkillIdList.Count() == 0 || skillIdIntersectList.Count() > 0)
                {
                    sosanh4.isSatisfied = true;
                }
                else
                {
                    sosanh4.isSatisfied = false;
                }
                matchingDetail.Add(sosanh4);

                // Benefit (nhieu TH) - 20
                IEnumerable<int> jobBenefitIdList = this.JobBenefitRepository.Get(s => s.JobID == jobId && s.IsDeleted == false).Select(s => s.BenefitID).AsEnumerable();
                IEnumerable<int> desiredBenefit = this.DesiredBenefitRepository.Get(s => s.JobSeekerID == profile.JobSeekerID && s.IsDeleted == false).Select(s => s.BenefitID).AsEnumerable();
                IEnumerable<int> benefitIdIntersectList = jobBenefitIdList.Intersect(desiredBenefit);
                SosanhItem sosanh5 = new SosanhItem();
                sosanh5.columnName = "Phúc lợi";
                if (jobBenefitIdList.Count() == 0)
                {
                    sosanh5.jobInfo = "Không hỗ trợ";
                }
                else
                {
                    sosanh5.jobInfo = this.BenefitRepository.GetByID(jobBenefitIdList.ElementAt(0)).Name;
                    for (int i = 1; i < jobBenefitIdList.Count(); i++)
                    {
                        sosanh5.jobInfo += "; " + this.BenefitRepository.GetByID(jobBenefitIdList.ElementAt(i)).Name;
                    }
                }
                if (desiredBenefit.Count() == 0)
                {
                    sosanh5.applicantInfo = "Không yêu cầu";
                }
                else
                {
                    sosanh5.applicantInfo = this.BenefitRepository.GetByID(desiredBenefit.ElementAt(0)).Name;
                    for (int i = 1; i < desiredBenefit.Count(); i++)
                    {
                        sosanh5.applicantInfo += "; " + this.BenefitRepository.GetByID(desiredBenefit.ElementAt(i)).Name;
                    }
                }
                if (jobBenefitIdList.Count() == 0 || benefitIdIntersectList.Count() > 0)
                {
                    sosanh5.isSatisfied = true;
                }
                else
                {
                    sosanh5.isSatisfied = false;
                }
                matchingDetail.Add(sosanh5);

                // Category - 10
                IEnumerable<int> jobCategoryIdList = this.JobCategoryRepository.Get(s => s.JobID == jobId && s.IsDeleted == false).Select(s => s.CategoryID).AsEnumerable();
                IEnumerable<int> expectedCategoryIdList = this.ExpectedCategoryRepository.Get(s => s.ProfileID == profileId && s.IsDeleted == false).Select(s => s.CategoryID).AsEnumerable();
                IEnumerable<int> categoryIdIntersectList = jobCategoryIdList.Intersect(expectedCategoryIdList);
                SosanhItem sosanh6 = new SosanhItem();
                sosanh6.columnName = "Ngành nghề";
                if (jobCategoryIdList.Count() == 0)
                {
                    sosanh6.jobInfo = "Không có";
                }
                else
                {
                    sosanh6.jobInfo = this.CategoryRepository.GetByID(jobCategoryIdList.ElementAt(0)).Name;
                    for (int i = 1; i < jobCategoryIdList.Count(); i++)
                    {
                        sosanh6.jobInfo += ", " + this.CategoryRepository.GetByID(jobCategoryIdList.ElementAt(i)).Name;
                    }
                }
                if (expectedCategoryIdList.Count() == 0)
                {
                    sosanh6.applicantInfo = "Không yêu cầu";
                }
                else
                {
                    sosanh6.applicantInfo = this.CategoryRepository.GetByID(expectedCategoryIdList.ElementAt(0)).Name;
                    for (int i = 1; i < expectedCategoryIdList.Count(); i++)
                    {
                        sosanh6.applicantInfo += "; " + this.CategoryRepository.GetByID(expectedCategoryIdList.ElementAt(i)).Name;
                    }
                }
                if (categoryIdIntersectList.Count() > 0)
                {
                    sosanh6.isSatisfied = true;
                }
                else
                {
                    sosanh6.isSatisfied = false;
                }
                matchingDetail.Add(sosanh6);

                // City - 10
                IEnumerable<int> jobCityIdList = this.JobCityRepository.Get(s => s.JobID == jobId && s.IsDeleted == false).Select(s => s.CityID).AsEnumerable();
                IEnumerable<int> expectedCityIdList = this.ExpectedCityRepository.Get(s => s.ProfileID == profileId && s.IsDeleted == false).Select(s => s.CityID).AsEnumerable();
                IEnumerable<int> cityIdIntersectList = jobCityIdList.Intersect(expectedCityIdList);
                SosanhItem sosanh7 = new SosanhItem();
                sosanh7.columnName = "Thành phố";
                if (jobCityIdList.Count() == 0)
                {
                    sosanh7.jobInfo = "Không có";
                }
                else
                {
                    sosanh7.jobInfo = this.CityRepository.GetByID(jobCityIdList.ElementAt(0)).Name;
                    for (int i = 1; i < jobCityIdList.Count(); i++)
                    {
                        sosanh7.jobInfo += "; " + this.CityRepository.GetByID(jobCityIdList.ElementAt(i)).Name;
                    }
                }
                if (expectedCityIdList.Count() == 0)
                {
                    sosanh7.applicantInfo = "Không có";
                }
                else
                {
                    sosanh7.applicantInfo = this.CityRepository.GetByID(expectedCityIdList.ElementAt(0)).Name;
                    for (int i = 1; i < expectedCityIdList.Count(); i++)
                    {
                        sosanh7.applicantInfo += "; " + this.CityRepository.GetByID(expectedCityIdList.ElementAt(i)).Name;
                    }
                }
                if (cityIdIntersectList.Count() > 0)
                {
                    sosanh7.isSatisfied = true;
                }
                else
                {
                    sosanh7.isSatisfied = false;
                }
                matchingDetail.Add(sosanh7);
            }

            return matchingDetail;
        }
    }
}
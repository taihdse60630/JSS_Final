using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using JobSearchingSystem.Models;
using System.Text.RegularExpressions;

namespace JobSearchingSystem.DAL
{
    public class JobUnitOfWork : UnitOfWork
    {
        public Boolean checkCity(IEnumerable<int> searchJobCities, IEnumerable<JJobCity> cityList)
        {
            Boolean flag = false;
            if (searchJobCities == null)
            {
                return true;
            }
            else
            {

                foreach (var item in cityList)
                {
                    foreach (var item2 in searchJobCities)
                    {
                        if (item2 == item.CityID)
                        {
                            flag = true;
                        }
                    }
                }
            }

            return flag;
        }

        public Boolean checkCategories(IEnumerable<int> searchJobCategories, IEnumerable<JJobCategory> categoryList)
        {
            Boolean flag = false;
            if (searchJobCategories == null)
            {
                return true;
            }
            else
            {

                foreach (var item in categoryList)
                {
                    foreach (var item2 in searchJobCategories)
                    {
                        if (item2 == item.CategoryID)
                        {
                            flag = true;
                        }
                    }
                }
            }

            return flag;
        }

        public string LocDau(string giatri)
        {
            try
            {
                Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
                string strRuler = giatri.Normalize(System.Text.NormalizationForm.FormD);
                strRuler = regex.Replace(strRuler, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D'); return Regex.Replace(strRuler, @"[^\w\.-]", " ");
            }
            catch { return "Co loi khi chuyen doi!"; }
        }


        public IEnumerable<JJobItem> FindJob(String searchString, double minSalary, int schoolLevel, IEnumerable<int> jobCities, IEnumerable<int> jobCategories)
        {

            var jobList = (from j in this.JobRepository.Get()
                           join c in this.CompanyInfoRepository.Get() on j.RecruiterID equals c.RecruiterID
                           join d in this.JobLevelRepository.Get() on j.JobLevel_ID equals d.JobLevel_ID
                           join f in this.SchoolLevelRepository.Get() on j.MinSchoolLevel_ID equals f.SchoolLevel_ID
                           join g in this.PurchaseJobPackageRepository.Get() on j.PurchaseJobPackageId equals g.PurchaseJobPackageID
                           join h in this.JobPackageRepository.Get() on g.JobPackageID equals h.JobPackageID
                           where ((j.IsPublic) && (j.StartedDate <= DateTime.Now) && (j.EndedDate >= DateTime.Now))
                           select new JJobItem()
                           {
                               JobID = j.JobID,
                               RecruiterID = j.RecruiterID,
                               JobTitle = j.JobTitle,
                               LogoURL = c.LogoURL,
                               JobLevelName = d.Name,
                               MinSalary = j.MinSalary.GetValueOrDefault(0),
                               MaxSalary = j.MaxSalary.GetValueOrDefault(0),
                               PostedDate = j.StartedDate,
                               SchoolLevel = f.LevelNum,
                               CompanyDescription = c.Description,
                               JobView = j.JobView,
                               isHightLight = h.IsHighlight

                           }
                   ).ToArray();

            for (int i = 0; i < jobList.Length; i++)
            {
                jobList[i].JobCities = (from a in this.JobCityRepository.Get()
                                        join b in this.CityRepository.Get() on a.CityID equals b.CityID
                                        where a.JobID == jobList[i].JobID
                                        select new JJobCity()
                                        {
                                            CityID = a.CityID,
                                            Name = b.Name
                                        }).ToArray();
            }

            for (int i = 0; i < jobList.Length; i++)
            {
                jobList[i].JobSkills = (from a in this.JobSkillRepository.Get()
                                        join b in this.SkillRepository.Get() on a.Skill_ID equals b.Skill_ID
                                        where a.JobID == jobList[i].JobID
                                        select new JJobSkill()
                                        {
                                            JobID = a.JobID,
                                            SkillID = b.Skill_ID,
                                            SkillTag = b.SkillTag
                                        }).ToArray();
            }

            for (int i = 0; i < jobList.Length; i++)
            {
                jobList[i].JobCategory = (from a in this.JobCategoryRepository.Get()
                                          join b in this.CategoryRepository.Get() on a.CategoryID equals b.CategoryID
                                          where a.JobID == jobList[i].JobID
                                          select new JJobCategory()
                                          {
                                              JobID = a.JobID,
                                              CategoryID = b.CategoryID,
                                              Name = b.Name
                                          }).ToArray();
            }

            if (!String.IsNullOrEmpty(searchString))
            {
                jobList = jobList.Where(s => LocDau(s.JobTitle.ToUpper()).Contains(LocDau(searchString.ToUpper()))
                                         && ((double)s.MinSalary >= minSalary) && (s.SchoolLevel <= schoolLevel) && checkCity(jobCities, s.JobCities)
                                         && checkCategories(jobCategories, s.JobCategory)
                    ).ToArray();
            }
            else
            {
                jobList = jobList.Where(s => ((double)s.MinSalary >= minSalary) && (s.SchoolLevel <= schoolLevel) && checkCity(jobCities, s.JobCities)
                                        && checkCategories(jobCategories, s.JobCategory)).ToArray();
            }

            return jobList.Reverse().ToArray();
        }

        public JJobItem GetJobDetail(int jobID)
        {
            Job jobDetail = this.JobRepository.GetByID(jobID);
            PurchaseJobPackage purchaseJobPackage = this.PurchaseJobPackageRepository.GetByID(jobDetail.PurchaseJobPackageId);
            JobPackage jobPackage = this.JobPackageRepository.GetByID(purchaseJobPackage.JobPackageID);
            jobDetail.JobView += Int64.Parse(jobPackage.ViewMultiNumber.GetValueOrDefault() <= 0 ? "1" : jobPackage.ViewMultiNumber.ToString());
            this.JobRepository.Update(jobDetail);
            Save();

            JJobItem job = (from j in this.JobRepository.Get()
                            join c in this.CompanyInfoRepository.Get() on j.RecruiterID equals c.RecruiterID
                            join d in this.JobLevelRepository.Get() on j.JobLevel_ID equals d.JobLevel_ID
                            join e in this.JobCategoryRepository.Get() on j.JobID equals e.JobID
                            join f in this.SchoolLevelRepository.Get() on j.MinSchoolLevel_ID equals f.SchoolLevel_ID
                            where ((j.JobID == jobID) && (c.IsVisible = true) && (j.IsPublic = true) && (j.StartedDate <= DateTime.Now) && (j.EndedDate >= DateTime.Now))
                            select new JJobItem()
                            {
                                JobID = j.JobID,
                                RecruiterID = j.RecruiterID,
                                JobTitle = j.JobTitle,
                                LogoURL = c.LogoURL,
                                JobLevelName = d.Name,
                                MinSalary = j.MinSalary,
                                MaxSalary = j.MaxSalary,
                                PostedDate = j.StartedDate,
                                SchoolLevel = f.LevelNum,
                                Company = c.Company,
                                Address = c.Address,
                                JobView = j.JobView,
                                JobDescription = j.JobDescription,
                                JobRequirement = j.JobRequirement,
                                CompanyDescription = c.Description
                            }).ToArray().First();

            job.ApplicantNumber = this.AppliedJobRepository.Get(filter: m => m.JobID == jobID).Count();

            job.JobCities = (from a in this.JobCityRepository.Get()
                             join b in this.CityRepository.Get() on a.CityID equals b.CityID
                             where a.JobID == job.JobID
                             select new JJobCity()
                             {
                                 CityID = a.CityID,
                                 Name = b.Name
                             }).ToArray();
            job.JobSkills = (from a in this.JobSkillRepository.Get()
                             join b in this.SkillRepository.Get() on a.Skill_ID equals b.Skill_ID
                             where a.JobID == job.JobID
                             select new JJobSkill()
                             {
                                 JobID = a.JobID,
                                 SkillID = b.Skill_ID,
                                 SkillTag = b.SkillTag
                             }).ToArray();
            job.JobCategory = (from a in this.JobCategoryRepository.Get()
                               join b in this.CategoryRepository.Get() on a.CategoryID equals b.CategoryID
                               where a.JobID == job.JobID
                               select new JJobCategory()
                               {
                                   JobID = a.JobID,
                                   CategoryID = b.CategoryID,
                                   Name = b.Name
                               }).ToArray();

            return job;
        }

        public bool IsJobExist(int jobID)
        {
            Job job = JobRepository.GetByID(jobID);
            if (job != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public IEnumerable<JAppliedJob> getAppliedJobList(string userID)
        {
            return (from a in this.AppliedJobRepository.Get()
                    join b in this.JobRepository.Get() on a.JobID equals b.JobID
                    where a.IsDeleted == false && a.JobSeekerID == userID
                    select new JAppliedJob()
                    {
                        JobName = b.JobTitle,
                        //CompanyName = b.Company,
                        AppliedDate = a.ApplyDate,
                        JobID = a.JobID,
                        JobSeekerID = a.JobSeekerID,
                        Status = a.Status,

                    }).AsEnumerable();
        }

        public int DeleteAppliedRequest(int jobId, string jobseekerId)
        {
            var listBeforeDelete = (from a in this.AppliedJobRepository.Get()
                                    where a.JobID == jobId && a.JobSeekerID == jobseekerId && a.IsDeleted == true
                                    select a).ToArray();


            AppliedJob appliedJob = AppliedJobRepository.Get(filter: m => m.JobID == jobId && m.JobSeekerID == jobseekerId).FirstOrDefault();
            appliedJob.IsDeleted = true;
            AppliedJobRepository.Update(appliedJob);
            Save();

            var list = (from a in this.AppliedJobRepository.Get()
                        where a.JobID == jobId && a.JobSeekerID == jobseekerId
                        select a).ToArray();
            return listBeforeDelete.Length - list.Length;
        }

        public IEnumerable<Profile> getJobSeekerProfile(string userID)
        {
            return ProfileRepository.Get(s => s.JobSeekerID == userID && s.IsDeleted == false).AsEnumerable();
        }

        public bool ApplyJob(int jobID, int profileID, string userID)
        {
            Job job = this.JobRepository.GetByID(jobID);
            Profile profile = this.ProfileRepository.GetByID(profileID);
            Jobseeker jobseeker = this.JobseekerRepository.GetByID(userID);

            if (job != null && profile != null && jobseeker != null)
            {
                AppliedJob appliedJob = this.AppliedJobRepository.Get(s => s.JobID == jobID && s.JobSeekerID == userID).FirstOrDefault();
                if (appliedJob != null)
                {
                    if (appliedJob.IsDeleted == true)
                    {
                        appliedJob.ProfileID = profileID;
                        appliedJob.IsDeleted = false;
                        this.AppliedJobRepository.Update(appliedJob);
                        this.Save();

                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    AppliedJob newAppliedJob = new AppliedJob();
                    newAppliedJob.JobID = jobID;
                    newAppliedJob.ProfileID = profileID;
                    newAppliedJob.JobSeekerID = userID;
                    newAppliedJob.ApplyDate = DateTime.Now;
                    newAppliedJob.MatchingPercent = this.Matching(profileID, jobID);
                    newAppliedJob.Status = 0;
                    newAppliedJob.IsDeleted = false;

                    this.AppliedJobRepository.Insert(newAppliedJob);
                    this.Save();

                    return true;
                }
            }

            return false;
        }

        //Change model information into Topic class
        public Job Model_Job(JobCreateModel model, int PurchaseJobPackageId)
        {
            Job temp = new Job();
            temp.RecruiterID = model.JobInfo.RecruiterID;
            temp.JobTitle = model.JobInfo.JobTitle;
            //temp.Company = model.JobInfo.Company;
            //temp.Address = model.JobInfo.Address;
            temp.MinSalary = model.JobInfo.MinSalary;
            temp.MaxSalary = model.JobInfo.MaxSalary;
            temp.JobDescription = model.JobInfo.JobDescription;
            temp.JobRequirement = model.JobInfo.JobRequirement;
            temp.JobLevel_ID = model.JobInfo.JobLevel_ID;
            temp.MinSchoolLevel_ID = model.JobInfo.MinSchoolLevel_ID;
            temp.JobView = model.JobInfo.JobView;
            temp.StartedDate = DateTime.Now;
            temp.EndedDate = DateTime.Now.AddDays(30);
            temp.IsPublic = model.JobInfo.IsPublic;
            temp.PurchaseJobPackageId = PurchaseJobPackageId;
            return temp;
        }

        //Create new job
        public bool CreateJob(JobCreateModel model, int PurchaseJobPackageId, string skill1, string skill2, string skill3)
        {
            //try
            //{
            this.JobRepository.Insert(Model_Job(model, PurchaseJobPackageId));
            this.Save();

            Job temp = this.JobRepository.Get(job => job.RecruiterID == model.JobInfo.RecruiterID && job.JobTitle == model.JobInfo.JobTitle).Last();

            //Add city
            foreach (int index in model.CitySelectList)
            {
                JobCity item = new JobCity();
                item.JobID = temp.JobID;
                item.CityID = index;
                this.JobCityRepository.Insert(item);
                this.Save();
            }

            //Add category
            foreach (int index in model.CategorySelectList)
            {
                JobCategory item = new JobCategory();
                item.JobID = temp.JobID;
                item.CategoryID = index;
                this.JobCategoryRepository.Insert(item);
                this.Save();
            }

            //Skill part
            if (skill1 != null)
            {
                Skill s1 = this.SkillRepository.Get(skill => skill.SkillTag == skill1).SingleOrDefault();
                if (s1 != null)
                {
                    JobSkill tempjs1 = new JobSkill();
                    tempjs1.JobID = temp.JobID;
                    tempjs1.Skill_ID = s1.Skill_ID;
                    tempjs1.IsDeleted = false;
                    this.JobSkillRepository.Insert(tempjs1);
                    this.Save();
                }
                else
                {
                    Skill temps1 = new Skill();
                    temps1.SkillTag = skill1;
                    temps1.IsDeleted = false;
                    this.SkillRepository.Insert(temps1);
                    this.Save();
                }
            }

            if (skill2 != null)
            {
                Skill s2 = this.SkillRepository.Get(skill => skill.SkillTag == skill2).SingleOrDefault();
                if (s2 != null)
                {
                    JobSkill tempjs2 = new JobSkill();
                    tempjs2.JobID = temp.JobID;
                    tempjs2.Skill_ID = s2.Skill_ID;
                    tempjs2.IsDeleted = false;
                    this.JobSkillRepository.Insert(tempjs2);
                    this.Save();
                }
                else
                {
                    Skill temps2 = new Skill();
                    temps2.SkillTag = skill2;
                    temps2.IsDeleted = false;
                    this.SkillRepository.Insert(temps2);
                    this.Save();
                }
            }
            if (skill3 != null)
            {
                Skill s3 = this.SkillRepository.Get(skill => skill.SkillTag == skill3).SingleOrDefault();
                if (s3 != null)
                {
                    JobSkill tempjs3 = new JobSkill();
                    tempjs3.JobID = temp.JobID;
                    tempjs3.Skill_ID = s3.Skill_ID;
                    tempjs3.IsDeleted = false;
                    this.JobSkillRepository.Insert(tempjs3);
                    this.Save();
                }
                else
                {
                    Skill temps3 = new Skill();
                    temps3.SkillTag = skill1;
                    temps3.IsDeleted = false;
                    this.SkillRepository.Insert(temps3);
                    this.Save();
                }
            }
            return true;
            //}
            //catch
            //{
            //    return false;
            //}
        }

        //Get list of job by recruiterID
        public IEnumerable<JobItem> GetJobByRecruiterID(string recruiterID)
        {
            try
            {
                List<JobItem> jobList = new List<JobItem>();
                foreach (Job job in this.JobRepository.Get(job => job.RecruiterID == recruiterID))
                {
                    if ((DateTime.Now.CompareTo(job.EndedDate) > 0) && (job.IsPublic == true))
                    {
                        job.IsPublic = false;
                        this.JobRepository.Update(job);
                        this.Save();
                    }

                    jobList.Add(new JobItem(job.JobID, job.JobTitle, job.StartedDate, job.EndedDate, job.IsPublic, job.AppliedJobs.Where(s => s.IsDeleted == false).Count()));
                }
                return jobList;
            }
            catch
            {
                return null;
            }
        }

        //Display a hidden job
        public bool Display(int jobID)
        {
            try
            {
                Job temp = this.JobRepository.Get(job => job.JobID == jobID).SingleOrDefault();
                if (temp == null) return false;
                temp.IsPublic = true;

                this.JobRepository.Update(temp);
                this.Save();

                return true;
            }
            catch
            {
                return false;
            }
        }

        //Hide a displayed job
        public bool Hide(int jobID)
        {
            try
            {
                Job temp = this.JobRepository.Get(job => job.JobID == jobID).SingleOrDefault();
                if (temp == null) return false;
                temp.IsPublic = false;

                this.JobRepository.Update(temp);
                this.Save();

                return true;
            }
            catch
            {
                return false;
            }
        }



        public bool CheckIsApplied(string userID, int jobID2)
        {
            IEnumerable<AppliedJob> appliedJob = AppliedJobRepository.Get(filter: s => s.JobID == jobID2 && s.JobSeekerID == userID && s.IsDeleted == false).AsEnumerable();
            if (appliedJob.ToArray().Length == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool CheckIfCanPostJob(string recruiterId)
        {
            Recruiter recruiter = this.RecruiterRepository.GetByID(recruiterId);
            if (recruiter != null)
            {
                IEnumerable<PurchaseJobPackage> purchaseJobPackage = this.PurchaseJobPackageRepository.Get(s => s.RecruiterID == recruiterId && s.IsApproved == true && DateTime.Now <= s.EndDate && s.IsDeleted == false).AsEnumerable();
                if (purchaseJobPackage.Count() > 0)
                {
                    int jobNumCanPost = 0;
                    foreach (PurchaseJobPackage item in purchaseJobPackage)
                    {
                        jobNumCanPost += this.JobPackageRepository.GetByID(item.JobPackageID).JobNumber;
                    }

                    int jobNumPosted = this.JobRepository.Get(s => s.RecruiterID == recruiterId && s.PurchaseJobPackageId != null).Count();

                    if (jobNumPosted <= jobNumCanPost - 1)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public int Matching(int profileId, int jobId)
        {
            int matchingPercent = 0;

            Profile profile = this.ProfileRepository.GetByID(profileId);
            Job job = this.JobRepository.GetByID(jobId);

            if (profile != null && job != null)
            {
                // MinSalary - MaxSalary Nullable - 20
                decimal expectedSalary = profile.ExpectedSalary;
                decimal? minSalary = job.MinSalary;
                decimal? maxSalary = job.MaxSalary;
                if (expectedSalary == 0
                    || (minSalary != null && maxSalary != null && minSalary <= expectedSalary && expectedSalary <= maxSalary)
                    || (minSalary != null && maxSalary == null && minSalary <= expectedSalary)
                    || (minSalary == null && maxSalary != null && expectedSalary <= maxSalary))
                {
                    matchingPercent += 20;
                }

                // JobLevel_ID - 10
                JobLevel expectedJobLevel = this.JobLevelRepository.GetByID(profile.ExpectedJobLevel_ID);
                JobLevel jobLevel = this.JobLevelRepository.GetByID(job.JobLevel_ID);
                if (expectedJobLevel != null && jobLevel != null)
                {
                    if (jobLevel.LevelNum >= expectedJobLevel.LevelNum)
                    {
                        matchingPercent += 10;
                    }
                }

                // MinSchoolLevel_ID - 10
                SchoolLevel highestSchoolLevel = this.SchoolLevelRepository.GetByID(profile.HighestSchoolLevel_ID);
                SchoolLevel minSchoolLevel = this.SchoolLevelRepository.GetByID(job.MinSchoolLevel_ID);
                if (highestSchoolLevel != null && minSchoolLevel != null)
                {
                    if (highestSchoolLevel.LevelNum >= minSchoolLevel.LevelNum)
                    {
                        matchingPercent += 10;
                    }
                }

                // Skill (nhieu TH) - 20
                IEnumerable<int> jobSkillIdList = this.JobSkillRepository.Get(s => s.JobID == jobId && s.IsDeleted == false).Select(s => s.Skill_ID).AsEnumerable();
                IEnumerable<int> ownSkillIdList = this.OwnSkillRepository.Get(s => s.JobSeekerID == profile.JobSeekerID && s.IsDeleted == false).Select(s => s.Skill_ID).AsEnumerable();
                IEnumerable<int> skillIdIntersectList = jobSkillIdList.Intersect(ownSkillIdList);
                if (jobSkillIdList.Count() == 0)
                {
                    matchingPercent += 20;
                }
                else if (skillIdIntersectList.Count() > 0)
                {
                    matchingPercent += skillIdIntersectList.Count() * 20 / jobSkillIdList.Count();
                }

                // Benefit (nhieu TH) - 20
                IEnumerable<int> jobBenefitIdList = this.JobBenefitRepository.Get(s => s.JobID == jobId && s.IsDeleted == false).Select(s => s.BenefitID).AsEnumerable();
                IEnumerable<int> desiredBenefit = this.DesiredBenefitRepository.Get(s => s.JobSeekerID == profile.JobSeekerID && s.IsDeleted == false).Select(s => s.BenefitID).AsEnumerable();
                IEnumerable<int> benefitIdIntersectList = jobBenefitIdList.Intersect(desiredBenefit);
                if (jobBenefitIdList.Count() == 0)
                {
                    matchingPercent += 20;
                }
                if (benefitIdIntersectList.Count() > 0)
                {
                    matchingPercent += benefitIdIntersectList.Count() * 20 / jobBenefitIdList.Count();
                }

                // Category - 10
                IEnumerable<int> jobCategoryIdList = this.JobCategoryRepository.Get(s => s.JobID == jobId && s.IsDeleted == false).Select(s => s.CategoryID).AsEnumerable();
                IEnumerable<int> expectedCategoryIdList = this.ExpectedCategoryRepository.Get(s => s.ProfileID == profileId && s.IsDeleted == false).Select(s => s.CategoryID).AsEnumerable();
                IEnumerable<int> categoryIdIntersectList = jobCategoryIdList.Intersect(expectedCategoryIdList);
                if (categoryIdIntersectList.Count() > 0)
                {
                    matchingPercent += 10;
                }

                // City - 10
                IEnumerable<int> jobCityIdList = this.JobCityRepository.Get(s => s.JobID == jobId && s.IsDeleted == false).Select(s => s.CityID).AsEnumerable();
                IEnumerable<int> expectedCityIdList = this.ExpectedCityRepository.Get(s => s.ProfileID == profileId && s.IsDeleted == false).Select(s => s.CityID).AsEnumerable();
                IEnumerable<int> cityIdIntersectList = jobCityIdList.Intersect(expectedCityIdList);
                if (cityIdIntersectList.Count() > 0)
                {
                    matchingPercent += 10;
                }
            }

            return matchingPercent;
        }

        public IEnumerable<JJobItem> JobsOfRecruiter(string recruiterID)
        {
            var jobList = (from a in this.JobRepository.Get()
                           where (a.RecruiterID == recruiterID) && (a.IsPublic = true) && (a.StartedDate <= DateTime.Now) && (a.EndedDate >= DateTime.Now)
                           select new JJobItem()
                           {
                               JobID = a.JobID,
                               JobTitle = a.JobTitle,
                               PostedDate = a.StartedDate,
                               JobView = a.JobView,

                           }).ToArray();
            for (int i = 0; i < jobList.Length; i++)
            {
                jobList[i].JobCities = (from a in this.JobCityRepository.Get()
                                        join b in this.CityRepository.Get() on a.CityID equals b.CityID
                                        where a.JobID == jobList[i].JobID
                                        select new JJobCity()
                                        {
                                            CityID = a.CityID,
                                            Name = b.Name
                                        }).ToArray();
            }
            return jobList.Reverse();
        }

        public JRecruiter GetRecuiterInfo(string recruiterID)
        {
            return (from a in this.CompanyInfoRepository.Get()
                    where a.RecruiterID == recruiterID
                    select new JRecruiter()
                    {
                        LogoURL = a.LogoURL,
                        Company = a.Company,
                        Address = a.Address,
                        Phone = a.PhoneNumber,
                        CompanyDescription = a.Description

                    }).FirstOrDefault();
        }

        public List<string> AutoCompleteSkill(string skill)
        {
            var list = (from a in this.SkillRepository.Get()

                        select new { a.SkillTag }).ToArray();
            List<string> listSkill = new List<string>();
            foreach (var item in list)
            {
                listSkill.Add(item.SkillTag);
            }
            return listSkill;
        }
    }
}
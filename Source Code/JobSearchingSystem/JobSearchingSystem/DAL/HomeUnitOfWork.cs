using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using JobSearchingSystem.Models;

namespace JobSearchingSystem.DAL
{
    public class HomeUnitOfWork : UnitOfWork
    {
        public  IEnumerable<JJobItem> getAllJob ()
        {

            var jobList = (from j in this.JobRepository.Get()
                           join c in this.CompanyInfoRepository.Get() on j.RecruiterID equals c.RecruiterID
                           join d in this.JobLevelRepository.Get() on j.JobLevel_ID equals d.JobLevel_ID
                           join f in this.SchoolLevelRepository.Get() on j.MinSchoolLevel_ID equals f.SchoolLevel_ID
                           join g in this.PurchaseJobPackageRepository.Get() on j.PurchaseJobPackageId equals g.PurchaseJobPackageID
                           join h in this.JobPackageRepository.Get() on g.JobPackageID equals h.JobPackageID
                           where ((h.IsHomepagePosting) && (j.IsPublic == true) && (DateTime.Now.CompareTo(j.StartedDate) >= 0) && (DateTime.Now.CompareTo(j.EndedDate) <= 0))
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
                               CompanyDescription = c.Description,
                               JobView = j.JobView,
                               purchaseJobPackageId = j.PurchaseJobPackageId,
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


            


         return jobList.Reverse();
        }

        public IEnumerable<JJobItem> getAllJobSortByView()
        {

            var jobList = (from j in this.JobRepository.Get()
                           join c in this.CompanyInfoRepository.Get() on j.RecruiterID equals c.RecruiterID
                           join d in this.JobLevelRepository.Get() on j.JobLevel_ID equals d.JobLevel_ID
                           join f in this.SchoolLevelRepository.Get() on j.MinSchoolLevel_ID equals f.SchoolLevel_ID
                           join g in this.PurchaseJobPackageRepository.Get() on j.PurchaseJobPackageId equals g.PurchaseJobPackageID
                           join h in this.JobPackageRepository.Get() on g.JobPackageID equals h.JobPackageID
                           where ((h.IsHomepagePosting) && (j.IsPublic == true) && (DateTime.Now.CompareTo(j.StartedDate) >= 0) && (DateTime.Now.CompareTo(j.EndedDate) <= 0))
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
                               CompanyDescription = c.Description,
                               JobView = j.JobView,
                               purchaseJobPackageId = j.PurchaseJobPackageId,
                               isHightLight = h.IsHighlight

                           }
                 ).OrderByDescending(m => m.JobView).ToArray();

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





            return jobList;
        }
     
     

        public IEnumerable<City> getAllCities()
        {
            return CityRepository.Get();
        }

        public IEnumerable<Category> getAllCategories()
        {
            return CategoryRepository.Get();
        }

        public IEnumerable<SchoolLevel> getAllSchoolLevel()
        {
            return SchoolLevelRepository.Get();
        }

        internal IEnumerable<PurchaseAdvertise> getPurchaseAdvertise(string position)
        {
            return (from a in this.PurchaseAdvertiseRepository.Get() 
                        join b in this.AdvertiseRepository.Get() on a.AdvertiseID equals b.AdvertiseID
                        where a.IsApproved == true && a.IsVisible == true && a.IsDeleted == false 
                              && a.EndDate >= DateTime.Now && b.Position == position
                        select new PurchaseAdvertise()
                        {
                            PurchaseAdsID = a.PurchaseAdsID,
                            LogoUrl = a.LogoUrl,
                            LinkUrl = a.LinkUrl,
                            RecuiterID = a.RecuiterID
                        }
                        ).AsEnumerable();
        }

        public IEnumerable<JJobItem> getAllJobAreSorted(string type)
        {
            if ("new".Equals(type))
            {
                return getAllJob();
            }
            else
            {
                var jobList = getAllJobSortByView();
                jobList.OrderByDescending(m => m.JobView);
                return jobList;
            }            
            

        }
    }
}
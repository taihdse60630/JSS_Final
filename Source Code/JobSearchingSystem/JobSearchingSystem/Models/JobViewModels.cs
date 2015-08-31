using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace JobSearchingSystem.Models
{
    public class JJobItem 
    {
        public int JobID { get; set; }
        public string RecruiterID { get; set; }
        public string JobTitle { get; set; }
        public string LogoURL { get; set; }      
        public decimal? MinSalary { get; set; }
        public decimal? MaxSalary { get; set; }
        public string JobLevelName { get; set; }
        public string JobDescription { get; set; }
        public string JobRequirement { get; set; }
        public string CompanyDescription { get; set; }
        public int SchoolLevel { get; set; }
        public string Company { get; set; }
        public string Address { get; set; }
        public long JobView { get; set; }
        public int ApplicantNumber { get; set; }
        public bool isHightLight { get; set; }
        public int? purchaseJobPackageId { get; set; }
        //[DataType(DataType.Date)]
        public DateTime? PostedDate { get; set; }
        public IEnumerable<JJobCategory> JobCategory { get; set; }
        public IEnumerable<JJobCity> JobCities { get; set; }
        public IEnumerable<JJobSkill> JobSkills { get; set; }
        
    }

    public class JJobSkill
    {
        public int SkillID { get; set; }
        public int JobID { get; set; }
        public string SkillTag { get; set; }
    }
    public class JJobCity
    {
        public int JobID { get; set; }
        public int CityID { get; set; }
        public string Name { get; set; }

    }
    public class JJobCategory
    {
        public int JobID { get; set; }
        public int CategoryID { get; set; }
        public string Name { get; set; }
    }
    public class JFindViewModel
    {
        public String searchString { get; set; }
        public IEnumerable<JJobItem> jJobItem { get; set; }
        public IEnumerable<int> searchJobCities { get; set; }
        public IEnumerable<int> searchjobCategories { get; set; }
        public IEnumerable<SchoolLevel> schoolLevelList { get; set; }
        public IEnumerable<City> jobCities { get; set; }
        public IEnumerable<Category> jobCategories { get; set; }
        public IEnumerable<PurchaseAdvertise> purchaseAdvertiseTypeA { get; set; }
        public int schoolLevel { get; set; }
        public double minSalary { get; set; }
    }

    public class JJobDetailViewModel
    {
        public JJobItem Job { get; set; }
        public IEnumerable<Profile> profileList { get; set; }
        public Jobseeker jobSeeker { get; set; }
        public int profileID { get; set; }
        public int jobID { get; set; }
        public bool isApplied { get; set; }
        public bool isLogined { get; set; }
        public IEnumerable<Job> jobList { get; set; }
        
    }

    public class JJobsOfRecruiter
    {
        public IEnumerable<JJobItem> jobsOfRecruiter { get; set; }
        public JRecruiter recruiter { get; set; }
    }

    public class JRecruiter
    {
        public string RecruiterID { get; set; }
        public string LogoURL { get; set; }
        public string CompanyDescription { get; set; }      
        public string Company { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        //[DataType(DataType.Date)]
      
        public IEnumerable<JJobCategory> JobCategory { get; set; }
        public IEnumerable<JJobCity> JobCities { get; set; }
      
    }

    public class JAppliedJob
    {
        
        public string JobName { get; set; }
        public string CompanyName { get; set; }
        public DateTime? AppliedDate { get; set; }
        public DateTime? AcceptDate { get; set; }
        public int JobID { get; set; }
        public string JobSeekerID { get; set; }
        public string RecruiterID { get; set; }
        public int Status { get; set; }
        
    }

    public class AppliedJobViewModel
    {
        public IEnumerable<JAppliedJob> AppliedJobList { get; set; }
    }
}
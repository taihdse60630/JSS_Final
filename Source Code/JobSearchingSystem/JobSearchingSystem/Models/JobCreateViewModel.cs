using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using JobSearchingSystem.DAL;

namespace JobSearchingSystem.Models
{
    //CLASS CONTAIN CORE INFORMATION OF JOB (TABLE JOB)
    public class JobCoreInformation
    {
        //Properties
        public string JobTitle { get; set; }
        public string RecruiterID { get; set; }
        //public string Address { get; set; }
        //public string Company { get; set; }
        public Nullable<decimal> MinSalary { get; set; }
        public Nullable<decimal> MaxSalary { get; set; }
        public string JobDescription { get; set; }
        public string JobRequirement { get; set; }
        public int JobLevel_ID { get; set; }
        public int MinSchoolLevel_ID { get; set; }
        public int JobView { get; set; }
        public Nullable<DateTime> StartedDate { get; set; }
        public Nullable<DateTime> EndDate { get; set; }
        public bool IsPublic { get; set; }

        //Construtor (0 parameter)
        public JobCoreInformation()
        {
            this.JobTitle = "";
            //this.Address = "";
            this.RecruiterID = "1";
            //this.Company = "FPT Software";
            this.MinSalary = null;
            this.MaxSalary = null;
            this.JobDescription = "";
            this.JobRequirement = "";
            this.JobLevel_ID = 2;
            this.MinSchoolLevel_ID = 1;
            this.JobView = 0;
            this.StartedDate = null;
            this.EndDate = null;
            this.IsPublic = false;
        }
    }

    public class JobPackageSelectItem {
        public int PurchaseJobPackageID { get; set; }
        public string JobPackageName { get; set; }
        public int RemainJobNumber { get; set; }
    }

    //MODEL USED FOR CREATING NEW JOB
    public class JobCreateModel
    {
        public JobCoreInformation JobInfo { get; set; }
        public IEnumerable<JobLevel> JobLevelList { get; set; }
        public IEnumerable<SchoolLevel> SchoolLevelList { get; set; }
        public IEnumerable<City> CityList { get; set; }
        public IEnumerable<Category> CategoryList { get; set; }
        public IEnumerable<Skill> SkillList { get; set; }

        public IEnumerable<JobPackageSelectItem> JobPackageItemSelectItemList { get; set; }
        public List<int> CategorySelectList { get; set; }
        public List<int> CitySelectList { get; set; }
        public List<int> SkillSelectList { get; set; }

        //Constructor (0 parameter)
        public JobCreateModel()
        {
            JobInfo = new JobCoreInformation();
            JobLevelList = new List<JobLevel>();
            SchoolLevelList = new List<SchoolLevel>();
            SkillList = new List<Skill>();
            CityList = new List<City>();
            CategoryList = new List<Category>();
            JobPackageItemSelectItemList = new List<JobPackageSelectItem>();

            CategorySelectList = new List<int>();
            CitySelectList = new List<int>();
            SkillSelectList = new List<int>();
        }
    }

    public class JobItem
    {
        public int JobID { get; set; }
        public string JobTitle { get; set; }
        public Nullable<DateTime> PostedDate { get; set; }
        public Nullable<DateTime> EndedDate { get; set; }
        public bool IsPublic { get; set; }
        public int ApplicantCount { get; set; }

        public JobItem()
        {
            this.JobID = 1;
            this.JobTitle = "";
            this.PostedDate = DateTime.Now;
            this.EndedDate = DateTime.Now;
            this.IsPublic = false;
            this.ApplicantCount = 0;
        }

        public JobItem(int jobID, string title, Nullable<DateTime> started, Nullable<DateTime> ended, bool IsPublic, int count)
        {
            this.JobID = jobID;
            this.PostedDate = started;
            this.EndedDate = ended;
            this.IsPublic = IsPublic;
            this.ApplicantCount = count;

            if (title.Length > 20)
            {
                this.JobTitle = title.Substring(0, 20) + "...";
            } else 
            {
                this.JobTitle = title;
            }

            title = null;
        }

    }
}
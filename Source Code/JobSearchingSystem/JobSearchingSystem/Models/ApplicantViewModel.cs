using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JobSearchingSystem.Models
{
    public class JobseekerList
    {
        public IEnumerable<ApplicantItem> jobseekerList { get; set; }
    }
    public class ApplicantItem
    {
        public string ApplicantID { get; set; }
        public string ApplicantName { get; set; }
        public string ExpectedJob { get; set; }
        public DateTime AppliedDate { get; set; }       
        public int Status { get; set; }
        public int MatchingPercent { get; set; }
        public IEnumerable<Profile> ProfileList { get; set; }
        public IEnumerable<int> MatchingPercentList { get; set; }
        public int YearOfExperience { get; set; }
        public string[] ExpectedLocation { get; set; }
        public string ApplicantUsername { get; set; }

        public ApplicantItem()
        {
            this.ApplicantID = "";
            this.ApplicantName = "";
            this.ExpectedJob = "";
            this.AppliedDate = DateTime.Now;
            this.MatchingPercent = 0;
            this.Status = 0;
        }

        public ApplicantItem(string applicantID, string applicantName, string currentJob, DateTime appliedDate, int percent, int status)
        {
            this.ApplicantID = applicantID;
            this.ApplicantName = applicantName;
            this.ExpectedJob = currentJob;
            this.AppliedDate = appliedDate;
            this.MatchingPercent = percent;
            this.Status = status;
        }
        

            
        public ApplicantItem(string applicantID, string applicantName, string currentJob, DateTime appliedDate, int status)
        {
            this.ApplicantID = applicantID;
            this.ApplicantName = applicantName;
            this.ExpectedJob = currentJob;
            this.AppliedDate = appliedDate;           
            this.Status = status;
        }


        public ApplicantItem(string applicantID, string applicantName, string currentJob, DateTime appliedDate, int percent, int status, string username)
        {
            this.ApplicantID = applicantID;
            this.ApplicantName = applicantName;
            this.ExpectedJob = currentJob;
            this.AppliedDate = appliedDate;
            this.MatchingPercent = percent;
            this.Status = status;
            this.ApplicantUsername = username;
        }

    }
}
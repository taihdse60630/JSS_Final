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

    public class SosanhItem
    {
        public string jobInfo { get; set; }
        public bool isSatisfied { get; set; }
        public string columnName { get; set; }
        public string applicantInfo { get; set; }
    }

    public class ApplicantItem
    {
        public string ApplicantID { get; set; }
        public string ProfileName { get; set; }
        public string ExpectedJob { get; set; }
        public DateTime AppliedDate { get; set; }       
        public int Status { get; set; }
        public int MatchingPercent { get; set; }
        public IEnumerable<Profile> ProfileList { get; set; }
        public IEnumerable<int> MatchingPercentList { get; set; }
        public int YearOfExperience { get; set; }
        public string[] ExpectedLocation { get; set; }
        public string ApplicantUsername { get; set; }
        public string ApplicantFullName { get; set; }
        public int ProfileID { get; set; }

        public ApplicantItem()
        {
            this.ApplicantID = "";
            this.ProfileName = "";
            this.ExpectedJob = "";
            this.AppliedDate = DateTime.Now;
            this.MatchingPercent = 0;
            this.Status = 0;
        }

        public ApplicantItem(string applicantID, string profileName, string currentJob, DateTime appliedDate, int percent, int status)
        {
            this.ApplicantID = applicantID;
            this.ProfileName = profileName;
            this.ExpectedJob = currentJob;
            this.AppliedDate = appliedDate;
            this.MatchingPercent = percent;
            this.Status = status;
        }



        public ApplicantItem(string applicantID, string profileName, string currentJob, DateTime appliedDate, int status)
        {
            this.ApplicantID = applicantID;
            this.ProfileName = profileName;
            this.ExpectedJob = currentJob;
            this.AppliedDate = appliedDate;           
            this.Status = status;
        }


        public ApplicantItem(string applicantID, string profileName, string currentJob, DateTime appliedDate, int percent, int status, string applicantUsername)
        {
            this.ApplicantID = applicantID;
            this.ProfileName = profileName;
            this.ExpectedJob = currentJob;
            this.AppliedDate = appliedDate;
            this.MatchingPercent = percent;
            this.Status = status;
            this.ApplicantUsername = applicantUsername;
        }

        public ApplicantItem(string applicantID, string profileName, string currentJob, DateTime appliedDate, int percent, int status, string applicantUsername, string applicantFullName, int profileId)
        {
            this.ApplicantID = applicantID;
            this.ProfileName = profileName;
            this.ExpectedJob = currentJob;
            this.AppliedDate = appliedDate;
            this.MatchingPercent = percent;
            this.Status = status;
            this.ApplicantUsername = applicantUsername;
            this.ApplicantFullName = applicantFullName;
            this.ProfileID = profileId;
        }
    }
}
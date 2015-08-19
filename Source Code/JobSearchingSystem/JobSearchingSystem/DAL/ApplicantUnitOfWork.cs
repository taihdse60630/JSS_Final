using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using JobSearchingSystem.Models;

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
                listApplicant.Add(new ApplicantItem(item.JobSeekerID, item.Profile.Name, item.Profile.ExpectedPosition, item.ApplyDate, item.MatchingPercent, item.Status));
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

    }
}
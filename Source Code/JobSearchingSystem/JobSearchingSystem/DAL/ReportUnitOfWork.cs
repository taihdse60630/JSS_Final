using JobSearchingSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JobSearchingSystem.DAL
{
    public class ReportUnitOfWork:UnitOfWork
    {

        public bool createReport(string reportContent, string senderId, string refrenceLink)
        {
            int size = ReportRepository.Get().ToArray().Length;
            Report report = new Report();
            report.Report_content = reportContent;
            report.CreatedDate = DateTime.Now;
            report.SenderID = senderId;
            report.ReferenceLink = refrenceLink;
            ReportRepository.Insert(report);
            Save();
            if (size < ReportRepository.Get().ToArray().Length)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public IEnumerable<JReport> GetAllReport()
        {
            return (from a in this.ReportRepository.Get()
                    join b in this.AspNetUserRepository.Get() on a.SenderID equals b.Id
                    where a.IsDeleted == false
                    select new JReport()
                    {
                        ReportID = a.ReportID,
                        Report_content = a.Report_content,
                        CreatedDate = a.CreatedDate,
                        SenderID = a.SenderID,
                        SenderUser = b.UserName,
                        IsSolved = a.IsSolved

                    }).AsEnumerable();
        }

        public void DeleteReport(int reportID)
        {
            Report report = ReportRepository.GetByID(reportID);
            report.IsDeleted = true;
            ReportRepository.Update(report);
            Save();
        }
    }
}
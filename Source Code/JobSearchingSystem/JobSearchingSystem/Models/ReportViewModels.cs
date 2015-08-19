using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JobSearchingSystem.Models
{
    public class ReportViewModels
    {
        public IEnumerable<JReport> reportList { get; set; }
    }

    public class JReport
    {
        public int ReportID { get; set; }
        public string Report_content { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string SenderID { get; set; }
        public bool IsSolved { get; set; }
        public string ResolvedUserID { get; set; }
        public string SenderUser { get; set; }
    }
}
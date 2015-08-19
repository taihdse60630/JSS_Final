using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JobSearchingSystem.Models
{
    public class JobPackageViewModels
    {
        public IEnumerable<JobPackage> jobPackageList { get; set; }
        public int packageQuantity { get;set; }
        public int jobPackageID { get; set; }
        
    }



}
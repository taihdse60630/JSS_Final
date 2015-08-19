using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JobSearchingSystem.Models
{
    public class JPurchaseJobPackage
    {
        public int PurchaseJobPackageID { get; set; }
        public string RecruiterID { get; set; }
        public int JobPackageID { get; set; }
        public bool? IsApproved { get; set; }
        public string RecruiterName { get; set; }
        public string JobPackageName { get; set; }
        public DateTime PurchaseDate { get; set; }
        
    }
    public class JobPackageRequestViewModels
    {
        public IEnumerable<JPurchaseJobPackage> purchaseJobPackageList { get; set; }

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JobSearchingSystem.Models
{
    public class InvoiceVIewModels
    {
        public JobPackage jobPackage { get; set; }
        public int packageQuantity { get; set; }
        public int jobPackageID { get; set; }
    }
}
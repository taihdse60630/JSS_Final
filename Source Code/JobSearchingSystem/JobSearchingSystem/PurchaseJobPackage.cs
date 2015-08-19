//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace JobSearchingSystem
{
    using System;
    using System.Collections.Generic;
    
    public partial class PurchaseJobPackage
    {
        public PurchaseJobPackage()
        {
            this.Jobs = new HashSet<Job>();
        }
    
        public int PurchaseJobPackageID { get; set; }
        public string RecruiterID { get; set; }
        public int JobPackageID { get; set; }
        public System.DateTime PurchasedDate { get; set; }
        public System.DateTime EndDate { get; set; }
        public Nullable<bool> IsApproved { get; set; }
        public string StaffID { get; set; }
        public bool IsDeleted { get; set; }
    
        public virtual ICollection<Job> Jobs { get; set; }
        public virtual JobPackage JobPackage { get; set; }
        public virtual Recruiter Recruiter { get; set; }
        public virtual Staff Staff { get; set; }
    }
}
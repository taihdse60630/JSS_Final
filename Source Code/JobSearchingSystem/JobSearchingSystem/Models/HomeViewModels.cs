using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JobSearchingSystem.Models
{
    
    public class HIndexViewModel
    {
        public IEnumerable<JJobItem> jobList { get; set; }
        public String searchString { get; set; }
        public IEnumerable<City> jobCities { get; set; }
        public IEnumerable<Category> jobCategories { get; set; }
        public IEnumerable<SchoolLevel> schoolLevelList { get; set; }
        public IEnumerable<int> searchJobCities { get; set; }
        public IEnumerable<int> searchJobCategories { get; set; }
        public IEnumerable<PurchaseAdvertise> purchaseAdvertiseTypeA { get; set; }
        public IEnumerable<PurchaseAdvertise> purchaseAdvertiseTypeB { get; set; }
        public IEnumerable<PurchaseAdvertise> purchaseAdvertiseTypeC { get; set; }

        public int schoolLevel { get; set; }
        public double minSalary { get; set; }

    }
}
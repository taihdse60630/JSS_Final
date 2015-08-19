using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JobSearchingSystem.Models
{
    public class AdvertiseRequestListViewModels
    {
        public IEnumerable<JPurchaseAdvertise> advertiseRequestList { get; set; }
        public string recruiterName { get; set; }
    }
    public class JPurchaseAdvertise
    {
        public int PurchaseAdsID { get; set; }
        public int AdvertiseID { get; set; }
        public string RecruiterID { get; set; }
        public string RecruiterName { get; set; }
        public bool isVisible { get; set; }
        public bool? isApproved { get; set; }
        public string AdvertiseName { get; set; }
        public string Position { get; set; }
        public decimal Cost { get; set; }
        public DateTime PurchaseDate { get; set; }
    }
    public class AdvertiseViewModels
    {
        public IEnumerable<Advertise> advertiseList { get; set; }
        public int advertiseID { get; set; }
    }

    public class AdvertiseInvoiceViewModels
    {
        public Advertise purchaseAdvertise { get; set; }
        public int advertiseID { get; set; }
        public string logo { get; set; }
    }

    public class ImageModel
    {
        public string imageUrl { get; set; }
        public ImageModel(string imageUrl)
        {
            this.imageUrl = imageUrl;
        }
     
    }
}
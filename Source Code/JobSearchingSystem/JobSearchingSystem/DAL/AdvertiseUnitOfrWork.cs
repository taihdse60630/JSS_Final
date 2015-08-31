using JobSearchingSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace JobSearchingSystem.DAL
{
    public class AdvertiseUnitOfrWork:UnitOfWork
    {

        public IEnumerable<Advertise> GetAllAdvertise()
        {
            return AdvertiseRepository.Get();
        }

        public bool CreateAdvertiseRequest(string userID, int advertiseID, string imageUrl)
        {
            try {
                CompanyInfo companyInfo = CompanyInfoRepository.GetByID(userID);
                PurchaseAdvertise purchaseAdvertise = new PurchaseAdvertise();
                purchaseAdvertise.AdvertiseID = advertiseID;
                purchaseAdvertise.LogoUrl = imageUrl;
                purchaseAdvertise.RecuiterID = userID;
                purchaseAdvertise.PurchasedDate = DateTime.Now;
                purchaseAdvertise.EndDate = DateTime.Now.AddDays(30);
                purchaseAdvertise.IsVisible = false;
                purchaseAdvertise.IsApproved = null;
                purchaseAdvertise.LinkUrl = "";

                PurchaseAdvertiseRepository.Insert(purchaseAdvertise);
                Save();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
      
        }

        public IEnumerable<JPurchaseAdvertise> GetAllAdvertiseRequest()
        {
            return (from a in this.PurchaseAdvertiseRepository.Get()
                    join b in this.AspNetUserRepository.Get() on a.RecuiterID equals b.Id
                    join c in this.AdvertiseRepository.Get() on a.AdvertiseID equals c.AdvertiseID
                    where a.IsDeleted == false
                    select new JPurchaseAdvertise
                    {
                        PurchaseAdsID = a.PurchaseAdsID,
                        RecruiterID = a.RecuiterID,
                        RecruiterName = b.UserName,
                        AdvertiseID = a.AdvertiseID,
                        AdvertiseName = c.Name,
                        isApproved = a.IsApproved
                    }).AsEnumerable();
        }

        public void AccepMultiAdvertise(List<int> listAccept, string userID)
        {
            foreach (var item in listAccept)
            {
                int PurchaseAdsID = item;
                PurchaseAdvertise purchaseAdvertise = PurchaseAdvertiseRepository.GetByID(PurchaseAdsID);
                purchaseAdvertise.IsApproved = true;
                purchaseAdvertise.IsVisible = true;
                purchaseAdvertise.PurchasedDate = DateTime.Now;
                purchaseAdvertise.EndDate = DateTime.Now.AddDays(30);
                purchaseAdvertise.ManagerID = userID;
                PurchaseAdvertiseRepository.Update(purchaseAdvertise);
                Save();
            }
        }

        public void DeleteMultiAdvertise(List<int> listAccept, string userID)
        {
            foreach (var item in listAccept)
            {
                int PurchaseAdsID = item;
                PurchaseAdvertise purchaseAdvertise = PurchaseAdvertiseRepository.GetByID(PurchaseAdsID);
                purchaseAdvertise.IsApproved = false;
                purchaseAdvertise.IsVisible = false;
                purchaseAdvertise.IsDeleted = true;
                purchaseAdvertise.ManagerID = userID;
                PurchaseAdvertiseRepository.Update(purchaseAdvertise);
                Save();
            }
        }

        public Advertise GetJobAdvertiseById(int advertiseID)
        {
            return AdvertiseRepository.GetByID(advertiseID);

        }

        public void RejectMultiAdvertise(List<int> listAccept, string userID)
        {
            foreach (var item in listAccept)
            {
                int PurchaseAdsID = item;
                PurchaseAdvertise purchaseAdvertise = PurchaseAdvertiseRepository.GetByID(PurchaseAdsID);
                purchaseAdvertise.IsApproved = false;
                purchaseAdvertise.IsVisible = false;
                purchaseAdvertise.ManagerID = userID;
                PurchaseAdvertiseRepository.Update(purchaseAdvertise);
                Save();
            }
        }

        public IEnumerable<JPurchaseAdvertise> FindAdvertiseRequest(string packageType, string recruiterName)
        {
            var advertiseRequestList = (from a in this.PurchaseAdvertiseRepository.Get()
                    join b in this.AspNetUserRepository.Get() on a.RecuiterID equals b.Id
                    join c in this.AdvertiseRepository.Get() on a.AdvertiseID equals c.AdvertiseID
                    where a.IsDeleted == false
                    select new JPurchaseAdvertise
                    {
                        PurchaseAdsID = a.PurchaseAdsID,
                        RecruiterID = a.RecuiterID,
                        RecruiterName = b.UserName,
                        AdvertiseID = a.AdvertiseID,
                        AdvertiseName = c.Name,
                        isApproved = a.IsApproved
                    }).AsEnumerable();

            if ("all".Equals(packageType) && String.IsNullOrEmpty(recruiterName))
            {
                return advertiseRequestList;
            }
            else if ("all".Equals(packageType) && !String.IsNullOrEmpty(recruiterName))
            {

                advertiseRequestList = advertiseRequestList.Where(s => LocDau(s.RecruiterName.ToUpper()).Contains(LocDau(recruiterName.ToUpper()))).ToArray();
                return advertiseRequestList;

            }
            else if (!"all".Equals(packageType) && String.IsNullOrEmpty(recruiterName))
            {
                advertiseRequestList = advertiseRequestList.Where(s => s.AdvertiseName.ToUpper() == packageType.ToUpper()).ToArray();
                return advertiseRequestList;
            }
            else
            {
                advertiseRequestList = advertiseRequestList.Where(s => LocDau(s.RecruiterName.ToUpper()).Contains(LocDau(recruiterName.ToUpper())) &&
                                                                   s.AdvertiseName.ToUpper() == packageType.ToUpper()).ToArray();
                return advertiseRequestList;
            }
    }

        public string LocDau(string giatri)
        {
            try
            {
                Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
                string strRuler = giatri.Normalize(System.Text.NormalizationForm.FormD);
                strRuler = regex.Replace(strRuler, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D'); return Regex.Replace(strRuler, @"[^\w\.-]", " ");
            }
            catch { return "Co loi khi chuyen doi!"; }
        }

        public IEnumerable<JPurchaseAdvertise> GetAllAdvertiseRequest(string userID)
        {
            return (from a in this.PurchaseAdvertiseRepository.Get()
                    join b in this.AspNetUserRepository.Get() on a.RecuiterID equals b.Id
                    join c in this.AdvertiseRepository.Get() on a.AdvertiseID equals c.AdvertiseID
                    where a.IsDeleted == false && a.RecuiterID == userID
                    select new JPurchaseAdvertise
                    {
                        PurchaseAdsID = a.PurchaseAdsID,
                        RecruiterID = a.RecuiterID,
                        RecruiterName = b.UserName,
                        AdvertiseID = a.AdvertiseID,
                        AdvertiseName = c.Name,
                        isApproved = a.IsApproved,
                        PurchaseDate = a.PurchasedDate
                    }).AsEnumerable().Reverse();
        }
    }
}
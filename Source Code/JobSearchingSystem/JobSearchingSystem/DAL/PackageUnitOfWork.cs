using JobSearchingSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JobSearchingSystem.DAL
{
    public class PackageUnitOfWork:UnitOfWork
    {
        public IEnumerable<JobPackage> GetAllPackage()
        {
            return JobPackageRepository.Get();
        }

        public JobPackage GetJobPackageById(int jobPackageId)
        {
            return JobPackageRepository.GetByID(jobPackageId);
        }

        public bool CreateJobPackageRequest(string userID, int packageID, int quantity)
        {
            var length = PurchaseJobPackageRepository.Get().ToArray().Length;
            PurchaseJobPackage purchaseJobPackage = null;
            //try{
                for (int i = 0; i < quantity; i++)
                {
                    purchaseJobPackage = new PurchaseJobPackage();
                    purchaseJobPackage.JobPackageID = packageID;
                    purchaseJobPackage.RecruiterID = userID;
                    purchaseJobPackage.PurchasedDate = DateTime.Now;
                    purchaseJobPackage.IsApproved = null;
                    purchaseJobPackage.EndDate = DateTime.Now.AddDays(30);

               
                        PurchaseJobPackageRepository.Insert(purchaseJobPackage);
                        Save();
                        
                    }
              var lengthAfterRequest = PurchaseJobPackageRepository.Get().ToArray().Length;
                    if(lengthAfterRequest > length){
                        return true;
                    }else{
                        return false;
                    }
                    //return true;
                 }

          
                //catch (Exception e)
                //{
                //    return false;
                //}
            
        

        public IEnumerable<JPurchaseJobPackage> GetAllJobPackageRequest()
        {
            IEnumerable<JPurchaseJobPackage> list = (from a in this.PurchaseJobPackageRepository.Get()
                                                     join b in this.AspNetUserRepository.Get() on a.RecruiterID equals b.Id
                                                     join c in this.JobPackageRepository.Get() on a.JobPackageID equals c.JobPackageID
                                                     where a.IsDeleted == false
                                                     select new JPurchaseJobPackage
                                                     {
                                                         PurchaseJobPackageID = a.PurchaseJobPackageID,
                                                         JobPackageName = c.Name,
                                                         RecruiterName = b.UserName,
                                                         IsApproved = a.IsApproved

                                                     }).AsEnumerable();
            return list;
        }

        public void AcceptJobPackageRequest(int JobPackageID, string userID)
        {
            PurchaseJobPackage purchaseJobPackage = PurchaseJobPackageRepository.GetByID(JobPackageID);
            purchaseJobPackage.IsApproved = true;
            purchaseJobPackage.StaffID = userID;
            PurchaseJobPackageRepository.Update(purchaseJobPackage);
            Save();
        }

        public void DeleteJobPackageRequest(int purchaseJobPackageID, string userID)
        {
            PurchaseJobPackage purchaseJobPackage = PurchaseJobPackageRepository.GetByID(purchaseJobPackageID);
            purchaseJobPackage.IsDeleted = true;
            purchaseJobPackage.StaffID = userID;
            PurchaseJobPackageRepository.Update(purchaseJobPackage);
            Save();
       
        }

        public void AccepMultitJobPackageRequest(List<int> listAccept, string userID)
        {
            foreach (var item in listAccept)
            {
                int JobPackageID = item;
                PurchaseJobPackage purchaseJobPackage = PurchaseJobPackageRepository.GetByID(JobPackageID);
                purchaseJobPackage.IsApproved = true;
                purchaseJobPackage.StaffID = userID;
                PurchaseJobPackageRepository.Update(purchaseJobPackage);
                Save();
            }
          
        }

        public void DeleteMultitJobPackageRequest(List<int> listDelete, string userID)
        {

            foreach (var item in listDelete)
            {
                int JobPackageID = item;
                PurchaseJobPackage purchaseJobPackage = PurchaseJobPackageRepository.GetByID(JobPackageID);
                purchaseJobPackage.IsDeleted = true;
                purchaseJobPackage.StaffID = userID;
                PurchaseJobPackageRepository.Update(purchaseJobPackage);
                Save();
            }
        }

        public void RejectMultiJobPackage(List<int> listReject, string userID)
        {
            foreach (var item in listReject)
            {
                int JobPackageID = item;
                PurchaseJobPackage purchaseJobPackage = PurchaseJobPackageRepository.GetByID(JobPackageID);
                purchaseJobPackage.IsApproved = false;
                purchaseJobPackage.StaffID = userID;
                PurchaseJobPackageRepository.Update(purchaseJobPackage);
                Save();
            }
        }

        public IEnumerable<JPurchaseJobPackage> GetAllJobPackageRequest(string userID)
        {
            IEnumerable<JPurchaseJobPackage> list = (from a in this.PurchaseJobPackageRepository.Get()
                                                     join b in this.AspNetUserRepository.Get() on a.RecruiterID equals b.Id
                                                     join c in this.JobPackageRepository.Get() on a.JobPackageID equals c.JobPackageID
                                                     where a.IsDeleted == false && a.RecruiterID == userID
                                                     select new JPurchaseJobPackage
                                                     {
                                                         PurchaseJobPackageID = a.PurchaseJobPackageID,
                                                         JobPackageName = c.Name,
                                                         RecruiterName = b.UserName,
                                                         IsApproved = a.IsApproved,
                                                         PurchaseDate = a.PurchasedDate
                                                     }).AsEnumerable();
            return list.Reverse();
        }
    }
}
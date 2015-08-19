using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JobSearchingSystem.DAL
{
    public class CompanyInfoUnitOfWork : UnitOfWork
    {
        public CompanyInfo GetCompanyInfo(string recuiterId) {
            CompanyInfo companyInfo = new CompanyInfo();
            if (recuiterId != null)
            {
                companyInfo = this.CompanyInfoRepository.Get(s => s.RecruiterID == recuiterId).FirstOrDefault(); 
            }
            return companyInfo;
        }

        public void CreateCompanyInfoCity(string recuiterId, int cityId)
        {
            CompanyInfoCity newCompanyInfoCity = new CompanyInfoCity();
            newCompanyInfoCity.RecuiterID = recuiterId;
            newCompanyInfoCity.CityID = cityId;
            newCompanyInfoCity.IsDeleted = false;
            this.CompanyInfoCityRepository.Insert(newCompanyInfoCity);
            this.Save();
        }

        public bool UpdateCompanyInfo(string recuiterId, string company, string address, string district, int cityId, string phoneNumber, string description, string logoURL)
        {
            if (!String.IsNullOrEmpty(company) && !String.IsNullOrEmpty(recuiterId))
            {
                CompanyInfo companyInfo = this.CompanyInfoRepository.Get(s => s.RecruiterID == recuiterId).FirstOrDefault();
                IEnumerable<CompanyInfoCity> companyInfoCity = this.CompanyInfoCityRepository.Get(s => s.RecuiterID == recuiterId).AsEnumerable();

                for (int i = 0; i < companyInfoCity.Count(); i++)
                {
                    CompanyInfoCity cic = companyInfoCity.ElementAt(i);
                    cic.IsDeleted = true;
                    this.CompanyInfoCityRepository.Update(cic);
                    this.Save();
                }

                if (companyInfo != null)
                {
                    companyInfo.Company = company;
                    companyInfo.Address = address;
                    companyInfo.District = district;
                    companyInfo.PhoneNumber = phoneNumber;
                    companyInfo.Description = description;
                    companyInfo.LogoURL = logoURL;
                    this.CompanyInfoRepository.Update(companyInfo);
                    this.Save();

                    if (companyInfoCity != null)
                    {
                        CompanyInfoCity cic = this.CompanyInfoCityRepository.Get(s => s.RecuiterID == recuiterId && s.CityID == cityId).FirstOrDefault();
                        if (cic != null)
                        {
                            cic.IsDeleted = false;
                            this.CompanyInfoCityRepository.Update(cic);
                            this.Save();
                        }
                        else
                        {
                            CreateCompanyInfoCity(recuiterId, cityId);
                        }
                    }
                    else if (companyInfoCity == null)
                    {
                        CreateCompanyInfoCity(recuiterId, cityId);
                    }
                    
                    return true;
                }
                else
                {
                    CompanyInfo newCompanyInfo = new CompanyInfo();
                    newCompanyInfo.RecruiterID = recuiterId;
                    newCompanyInfo.Company = company;
                    newCompanyInfo.Address = address;
                    newCompanyInfo.District = district;
                    newCompanyInfo.PhoneNumber = phoneNumber;
                    newCompanyInfo.Description = description;
                    newCompanyInfo.LogoURL = logoURL;
                    newCompanyInfo.IsVisible = false;
                    this.CompanyInfoRepository.Insert(newCompanyInfo);
                    this.Save();

                    CreateCompanyInfoCity(recuiterId, cityId);

                    return true;
                }
            }
            return false;
        }
    }
}
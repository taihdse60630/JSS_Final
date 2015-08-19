using JobSearchingSystem.DAL;
using JobSearchingSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JobSearchingSystem.Controllers
{
    public class CompanyInfoController : Controller
    {
        private CompanyInfoUnitOfWork companyInfoUnitOfWork = new CompanyInfoUnitOfWork();

        //
        // GET: /CompanyInfo/
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Update()
        {
            string userName = User.Identity.Name;
            AspNetUser user = companyInfoUnitOfWork.AspNetUserRepository.Get(s => s.UserName == userName).FirstOrDefault();

            CoInUpdateViewModel model = new CoInUpdateViewModel();
            CompanyInfo companyInfo = companyInfoUnitOfWork.GetCompanyInfo(user.Id);
            IEnumerable<City> cities = companyInfoUnitOfWork.CityRepository.Get(s => s.IsDeleted == false).OrderBy(s => s.Name).AsEnumerable();
            if (companyInfo != null)
            {
                model.logoURL = companyInfo.LogoURL;
                model.company = companyInfo.Company;
                model.address = companyInfo.Address;
                model.district = companyInfo.District;

                CompanyInfoCity cic = companyInfoUnitOfWork.CompanyInfoCityRepository.Get(s => s.RecuiterID == user.Id && s.IsDeleted == false).FirstOrDefault();
                if (cic != null)
                {
                    City city = companyInfoUnitOfWork.CityRepository.GetByID(cic.CityID);
                    if (city != null)
                    {
                        model.city = city.Name;
                    }
                }
                
                model.phoneNumber = companyInfo.PhoneNumber;
                model.description = companyInfo.Description;
            }
            model.recuiterId = user.Id;
            model.cities = cities;

            return View(model);
        }

        [HttpPost]
        public ActionResult Update(string recuiterId, string company,
                                    string address, string district, string cityId,
                                    string phoneNumber, string description, string logoURL)
        {
            if (!String.IsNullOrEmpty(company) && !String.IsNullOrEmpty(recuiterId) && !String.IsNullOrEmpty(cityId))
            {
                int cityIdNum = Int32.Parse(cityId);
                bool result = companyInfoUnitOfWork.UpdateCompanyInfo(recuiterId, company, address, district, cityIdNum, phoneNumber, description, logoURL);
                CoInUpdateViewModel model = new CoInUpdateViewModel();
                if (result)
                {
                    return RedirectToAction("Update");
                }
            }
            
            return RedirectToAction("Update");
        }


	}
}
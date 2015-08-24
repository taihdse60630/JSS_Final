using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using JobSearchingSystem.DAL;
using JobSearchingSystem.Models;

namespace JobSearchingSystem.Controllers
{
    [MessageFilter]
    public class ProfileController : Controller
    {
        private ProfileUnitOfWork profileUnitOfWork = new ProfileUnitOfWork();

        [Authorize(Roles = "Jobseeker")]
        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        [Authorize(Roles = "Jobseeker")]
        public ActionResult List()
        {
            ProListViewModel proListViewModel = new ProListViewModel();

            string jobseekerId = profileUnitOfWork.AspNetUserRepository.Get(s => s.UserName == User.Identity.Name).FirstOrDefault().Id;

            proListViewModel.proList = profileUnitOfWork.GetAllProList(jobseekerId);

            return View(proListViewModel);
        }

        [Authorize(Roles = "Recruiter")]
        public ActionResult Detail(string profileID)
        {
            ProDetailViewModel model = new ProDetailViewModel();

            if (!String.IsNullOrEmpty(profileID))
            {
                int profileIDNum = 0;
                try 
                {
                    profileIDNum = Int32.Parse(profileID);
                }
                catch (Exception)
                {
                    TempData["errormessage"] = "Lỗi dữ liệu!";
                    return RedirectToAction("Index", "Home");
                }

                model.profile = profileUnitOfWork.ProfileRepository.GetByID(profileIDNum);
                if (model.profile != null && model.profile.IsActive == true && model.profile.IsDeleted == false)
                {
                    AspNetUser recruiter = profileUnitOfWork.AspNetUserRepository.Get(ss => ss.UserName == User.Identity.Name).FirstOrDefault();
                    ViewProfile viewProfile = profileUnitOfWork.ViewProfileRepository.Get(s => s.RecruiterID == recruiter.Id && s.ProfileID == profileIDNum).FirstOrDefault();

                    if (viewProfile == null)
                    {
                        ViewProfile newViewProfile = new ViewProfile();
                        newViewProfile.RecruiterID = recruiter.Id;
                        newViewProfile.ProfileID = profileIDNum;
                        newViewProfile.ViewedDate = DateTime.Now;
                        profileUnitOfWork.ViewProfileRepository.Insert(newViewProfile);
                        profileUnitOfWork.Save();
                    }
                    else
                    {
                        viewProfile.ViewedDate = DateTime.Now;
                        profileUnitOfWork.ViewProfileRepository.Update(viewProfile);
                        profileUnitOfWork.Save();
                    }

                    model.cities = profileUnitOfWork.CityRepository.Get(filter: d => d.IsDeleted == false).OrderBy(d => d.Name).AsEnumerable();
                    model.schoolLevels = profileUnitOfWork.SchoolLevelRepository.Get(filter: d => d.IsDeleted == false).OrderByDescending(d => d.LevelNum).AsEnumerable();
                    model.languages = profileUnitOfWork.LanguageRepository.Get(filter: d => d.IsDeleted == false).OrderBy(d => d.Name).AsEnumerable();
                    model.levels = profileUnitOfWork.LevelRepository.Get(filter: d => d.IsDeleted == false).OrderByDescending(d => d.LevelNum).AsEnumerable();
                    model.jobLevels = profileUnitOfWork.JobLevelRepository.Get(filter: d => d.IsDeleted == false).OrderByDescending(d => d.LevelNum).AsEnumerable();
                    model.categories = profileUnitOfWork.CategoryRepository.Get(filter: d => d.IsDeleted == false).OrderBy(d => d.Name).AsEnumerable();
                    ExpectedCity expectedCity = profileUnitOfWork.ExpectedCityRepository.Get(filter: d => d.ProfileID == profileIDNum && d.IsDeleted == false).FirstOrDefault();
                    if (expectedCity != null)
                    {
                        model.expectedCity = expectedCity.CityID;
                    }

                    ExpectedCategory expectedCategory = profileUnitOfWork.ExpectedCategoryRepository.Get(filter: d => d.ProfileID == profileIDNum && d.IsDeleted == false).FirstOrDefault();
                    if (expectedCategory != null)
                    {
                        model.categoryID = expectedCategory.CategoryID;
                    }

                    model.contact = profileUnitOfWork.ContactRepository.GetByID(model.profile.JobSeekerID);
                    model.employmentHistory = profileUnitOfWork.EmploymentHistoryRepository.Get(s => s.ProfileID == profileIDNum && s.IsDeleted == false).LastOrDefault();
                    model.educationHistory = profileUnitOfWork.EducationHistoryRepository.Get(s => s.ProfileID == profileIDNum && s.IsDeleted == false).LastOrDefault();
                    model.referencePerson = profileUnitOfWork.ReferencePersonRepository.Get(s => s.ProfileID == profileIDNum && s.IsDeleted == false).LastOrDefault();

                    return View(model);
                }
                else
                {
                    TempData["errormessage"] = "Không tìm thấy thông tin hồ sơ!";
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                TempData["errormessage"] = "Dữ liệu không hợp lệ!";
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize(Roles = "Jobseeker")]
        public ActionResult Update(string profileID, string mode)
        {
            if ("create".Equals(mode))
            {
                AspNetUser user = profileUnitOfWork.AspNetUserRepository.Get(s => s.UserName == User.Identity.Name).FirstOrDefault();
                IEnumerable<Profile> profiles = profileUnitOfWork.ProfileRepository.Get(s => s.JobSeekerID == user.Id && s.IsDeleted == false).AsEnumerable();

                if (profiles.Count() >= 5)
                {
                    TempData["warningmessage"] = "Bạn chỉ được tạo tối đa 5 hồ sơ!";
                    return RedirectToAction("List");
                }
            }

            ProUpdateViewModel proUpdateViewModel = new ProUpdateViewModel();

            proUpdateViewModel.cities = profileUnitOfWork.CityRepository.Get(filter: d => d.IsDeleted == false).OrderBy(d => d.Name).AsEnumerable();
            proUpdateViewModel.schoolLevels = profileUnitOfWork.SchoolLevelRepository.Get(filter: d => d.IsDeleted == false).OrderByDescending(d => d.LevelNum).AsEnumerable();
            proUpdateViewModel.languages = profileUnitOfWork.LanguageRepository.Get(filter: d => d.IsDeleted == false).OrderBy(d => d.Name).AsEnumerable();
            proUpdateViewModel.levels = profileUnitOfWork.LevelRepository.Get(filter: d => d.IsDeleted == false).OrderByDescending(d => d.LevelNum).AsEnumerable();
            proUpdateViewModel.jobLevels = profileUnitOfWork.JobLevelRepository.Get(filter: d => d.IsDeleted == false).OrderByDescending(d => d.LevelNum).AsEnumerable();
            proUpdateViewModel.categories = profileUnitOfWork.CategoryRepository.Get(filter: d => d.IsDeleted == false).OrderBy(d => d.Name).AsEnumerable();

            proUpdateViewModel.contact = profileUnitOfWork.ContactRepository.GetByID(profileUnitOfWork.AspNetUserRepository.Get(s => s.UserName == User.Identity.Name).FirstOrDefault().Id);
            if (proUpdateViewModel.contact == null)
            {
                proUpdateViewModel.contact = new Contact();
                proUpdateViewModel.contact.DateofBirth = DateTime.Now;
            }

            proUpdateViewModel.commonInfoItem = new ProCommonInfoItem();
            if (!String.IsNullOrEmpty(profileID))
            {
                ViewBag.ButtonName = "Cập nhật";

                int profileIDNum = 0;
                try
                {
                    profileIDNum = Int32.Parse(profileID);
                }
                catch (Exception)
                {
                    return View(proUpdateViewModel);
                }

                Profile profile = profileUnitOfWork.ProfileRepository.GetByID(profileIDNum);
                if (profile != null)
                {
                    ViewBag.ButtonName = "Cập nhật";

                    proUpdateViewModel.commonInfoItem.profile = profile;

                    ExpectedCity expectedCity = profileUnitOfWork.ExpectedCityRepository.Get(filter: d => d.ProfileID == profileIDNum && d.IsDeleted == false).FirstOrDefault();
                    if (expectedCity != null){
                        proUpdateViewModel.commonInfoItem.expectedCity = expectedCity.CityID;
                    }

                    ExpectedCategory expectedCategory = profileUnitOfWork.ExpectedCategoryRepository.Get(filter: d => d.ProfileID == profileIDNum && d.IsDeleted == false).FirstOrDefault();
                    if (expectedCategory != null)
                    {
                        proUpdateViewModel.commonInfoItem.categoryID = expectedCategory.CategoryID;
                    }

                    EmploymentHistory employmentHistory = profileUnitOfWork.EmploymentHistoryRepository.Get(s => s.ProfileID == profileIDNum && s.IsDeleted == false).FirstOrDefault();
                    if (employmentHistory != null)
                    {
                        proUpdateViewModel.employmentHistory = employmentHistory;
                    }
                    else
                    {
                        proUpdateViewModel.employmentHistory = new EmploymentHistory();
                        proUpdateViewModel.employmentHistory.EmploymentHistoryID = -1;
                    }

                    EducationHistory educationHistory = profileUnitOfWork.EducationHistoryRepository.Get(s => s.ProfileID == profileIDNum && s.IsDeleted == false).FirstOrDefault();
                    if (educationHistory != null)
                    {
                        proUpdateViewModel.educationHistory = educationHistory;
                    }
                    else
                    {
                        proUpdateViewModel.educationHistory = new EducationHistory();
                        proUpdateViewModel.educationHistory.EducationHistoryID = -1;
                    }

                    ReferencePerson referencePerson = profileUnitOfWork.ReferencePersonRepository.Get(s => s.ProfileID == profileIDNum && s.IsDeleted == false).FirstOrDefault();
                    if (referencePerson != null)
                    {
                        proUpdateViewModel.referencePerson = referencePerson;
                    }
                    else
                    {
                        proUpdateViewModel.referencePerson = new ReferencePerson();
                        proUpdateViewModel.referencePerson.ReferencePersonID = -1;
                    }
                }
                else
                {
                    ViewBag.ButtonName = "Tạo mới";

                    proUpdateViewModel.employmentHistory = new EmploymentHistory();
                    proUpdateViewModel.employmentHistory.EmploymentHistoryID = -1;
                    proUpdateViewModel.educationHistory = new EducationHistory();
                    proUpdateViewModel.educationHistory.EducationHistoryID = -1;
                    proUpdateViewModel.referencePerson = new ReferencePerson();
                    proUpdateViewModel.referencePerson.ReferencePersonID = -1;
                }
            }
            else
            {
                ViewBag.ButtonName = "Tạo mới";

                proUpdateViewModel.employmentHistory = new EmploymentHistory();
                proUpdateViewModel.employmentHistory.EmploymentHistoryID = -1;
                proUpdateViewModel.educationHistory = new EducationHistory();
                proUpdateViewModel.educationHistory.EducationHistoryID = -1;
                proUpdateViewModel.referencePerson = new ReferencePerson();
                proUpdateViewModel.referencePerson.ReferencePersonID = -1;
            }

            return View(proUpdateViewModel);
        }

        [Authorize(Roles = "Jobseeker")]
        [HttpPost]       
        public ActionResult Update([Bind(Include = "FullName, Gender, MaritalStatus, Nationality, Address, DateofBirth, PhoneNumber, CityID, District, IsVisible")] 
                                        Contact contact,
                                   [Bind(Include = "ProfileID, Name, YearOfExperience, HighestSchoolLevel_ID, LanguageID, Level_ID, MostRecentCompany, MostRecentPosition, CurrentJobLevel_ID, ExpectedPosition, ExpectedJobLevel_ID, ExpectedSalary, Objectives")]
                                            Profile profile, string expectedCity, string categoryID,
                                   [Bind(Include = "EmploymentHistoryID, Position, Company, WorkingTime, Description")]EmploymentHistory employmentHistory,
                                   [Bind(Include = "EducationHistoryID, Subject, School, SchoolLevel_ID, Achievement")]EducationHistory educationHistory,
                                   [Bind(Include = "ReferencePersonID, ReferencePersonName, ReferencePersonPosition, ReferencePersonCompany, EmailAddress, ReferencePersonPhoneNumber")]ReferencePerson referencePerson,
                                   string ButtonName)
        {
            bool contactResult = UpdateContact(contact);
            if ("Tạo mới".Equals(ButtonName))
            {
                AspNetUser jobseeker = profileUnitOfWork.AspNetUserRepository.Get(s => s.UserName == User.Identity.Name && s.IsActive == true).LastOrDefault();
                if (jobseeker != null)
                {
                    Profile oldProfile = profileUnitOfWork.ProfileRepository.Get(s => s.Name == profile.Name && s.JobSeekerID == jobseeker.Id && s.IsDeleted == false).LastOrDefault();

                    if (oldProfile != null)
                    {
                        TempData["warningmessage"] = "Tên hồ sơ đã tồn tại, xin hãy tạo lại hồ sơ bằng tên khác!";
                        return RedirectToAction("Update");
                    }
                }
                
            }
            bool commonInfoResult = UpdateCommonInfo(profile, expectedCity, categoryID);
            bool employmentHistoryResult = false;
            bool educationHistoryResult = false;
            bool referencePersonResult = false;
            if (commonInfoResult) {
                Profile updatedProfile = profileUnitOfWork.ProfileRepository.Get(s => s.Name == profile.Name).LastOrDefault();
                if (updatedProfile != null)
                {
                    int percentStatus = 25;

                    int profileID = updatedProfile.ProfileID;

                    employmentHistoryResult = UpdateEmploymentHistory(employmentHistory, profileID);
                    if (employmentHistoryResult) { percentStatus += 25; }
                    educationHistoryResult = UpdateEducationHistory(educationHistory, profileID);
                    if (educationHistoryResult) { percentStatus += 25; } 
                    referencePersonResult = UpdateReferencePerson(referencePerson, profileID);
                    if (referencePersonResult) { percentStatus += 25; }

                    updatedProfile.PercentStatus = percentStatus;
                    profileUnitOfWork.ProfileRepository.Update(updatedProfile);
                    profileUnitOfWork.Save();

                    TempData["successmessage"] = "Tạo/Cập nhật hồ sơ thành công.";
                    return RedirectToAction("List");
                }

                TempData["errormessage"] = "Tạo/Cập nhật hồ sơ thất bại!";
                return RedirectToAction("Update");
            }

            TempData["errormessage"] = "Tạo/Cập nhật hồ sơ thất bại!";
            return RedirectToAction("Update", new { profileID = profile.ProfileID });
        }

        [Authorize(Roles = "Jobseeker")]
        private bool UpdateContact(Contact contact)
        {
            if (!String.IsNullOrEmpty(contact.FullName)
                && !String.IsNullOrEmpty(contact.PhoneNumber))
            {
                contact.UserID = profileUnitOfWork.AspNetUserRepository.Get(s => s.UserName == User.Identity.Name).FirstOrDefault().Id;

                bool result = this.profileUnitOfWork.UpdateContact(contact);

                if (result)
                {
                    return true;
                }
            }

            return false;
        }

        [Authorize(Roles = "Jobseeker")]
        private bool UpdateCommonInfo(Profile profile, string expectedCity, string categoryID)
        {
            if (!String.IsNullOrEmpty(profile.Name)
                && !String.IsNullOrEmpty(profile.ExpectedPosition)
                && !String.IsNullOrEmpty(profile.Objectives)
                && !String.IsNullOrEmpty(expectedCity)
                && !String.IsNullOrEmpty(categoryID))
            {
                int expectedCityNum = 0;
                int categoryIDNum = 0;

                try
                {
                    expectedCityNum = Int32.Parse(expectedCity);
                    categoryIDNum = Int32.Parse(categoryID);
                }
                catch (Exception)
                {
                    return false;
                }

                if (profile.CurrentJobLevel_ID == -1)
                {
                    profile.CurrentJobLevel_ID = null;
                }
                profile.CreatedTime = DateTime.Now;
                profile.UpdatedTime = profile.CreatedTime;
                profile.PercentStatus = 25;
                string userId = profileUnitOfWork.AspNetUserRepository.Get(filter: d => d.UserName == User.Identity.Name).FirstOrDefault().Id;
                profile.JobSeekerID = userId;
                profile.IsActive = false;
                profile.IsDeleted = false;

                bool result = this.profileUnitOfWork.UpdateCommonInfo(profile, expectedCityNum, categoryIDNum);

                if (result)
                {
                    return true;
                }
            }

            return false;
        }

        [Authorize(Roles = "Jobseeker")]
        private bool UpdateEmploymentHistory(EmploymentHistory employmentHistory, int profileID)
        {
            if (!String.IsNullOrEmpty(employmentHistory.Position)
                && !String.IsNullOrEmpty(employmentHistory.Company))
            {
                if (employmentHistory.EmploymentHistoryID == -2)
                {
                    return false;
                }
                else if (employmentHistory.EmploymentHistoryID == -1)
                {
                    // Add new
                    employmentHistory.ProfileID = profileID;
                    employmentHistory.IsDeleted = false;
                    profileUnitOfWork.EmploymentHistoryRepository.Insert(employmentHistory);
                    profileUnitOfWork.Save();
                }
                else
                {
                    // Update
                    employmentHistory.ProfileID = profileID;
                    employmentHistory.IsDeleted = false;
                    profileUnitOfWork.EmploymentHistoryRepository.Update(employmentHistory);
                    profileUnitOfWork.Save();
                }

                return true;
            }

            return false;
        }

        [Authorize(Roles = "Jobseeker")]
        private bool UpdateEducationHistory(EducationHistory educationHistory, int profileID)
        {
            if (!String.IsNullOrEmpty(educationHistory.Subject)
                && !String.IsNullOrEmpty(educationHistory.School))
            {
                if (educationHistory.EducationHistoryID == -2)
                {
                    return false;
                }
                else if (educationHistory.EducationHistoryID == -1)
                {
                    // Add new
                    educationHistory.ProfileID = profileID;
                    educationHistory.IsDeleted = false;
                    profileUnitOfWork.EducationHistoryRepository.Insert(educationHistory);
                    profileUnitOfWork.Save();
                }
                else
                {
                    // Update
                    educationHistory.ProfileID = profileID;
                    educationHistory.IsDeleted = false;
                    profileUnitOfWork.EducationHistoryRepository.Update(educationHistory);
                    profileUnitOfWork.Save();
                }

                return true;
            }

            return false;
        }

        [Authorize(Roles = "Jobseeker")]
        private bool UpdateReferencePerson(ReferencePerson referencePerson, int profileID)
        {
            if (!String.IsNullOrEmpty(referencePerson.ReferencePersonName)
                && !String.IsNullOrEmpty(referencePerson.ReferencePersonPosition)
                && !String.IsNullOrEmpty(referencePerson.ReferencePersonCompany)
                && !String.IsNullOrEmpty(referencePerson.EmailAddress))
            {
                if (referencePerson.ReferencePersonID == -2)
                {
                    return false;
                }
                else if (referencePerson.ReferencePersonID == -1)
                {
                    // Add new
                    referencePerson.ProfileID = profileID;
                    referencePerson.IsDeleted = false;
                    profileUnitOfWork.ReferencePersonRepository.Insert(referencePerson);
                    profileUnitOfWork.Save();
                }
                else
                {
                    // Update
                    referencePerson.ProfileID = profileID;
                    referencePerson.IsDeleted = false;
                    profileUnitOfWork.ReferencePersonRepository.Update(referencePerson);
                    profileUnitOfWork.Save();
                }

                return true;
            }

            return false;
        }

        [Authorize(Roles = "Jobseeker")]
        [HttpPost]
        public ActionResult ActiveProfile(string activeProfileId, string activeStatus)
        {
            if (!String.IsNullOrEmpty(activeProfileId)
                && !String.IsNullOrEmpty(activeStatus))
            {
                int profileIDNum;

                try
                {
                    profileIDNum = Int32.Parse(activeProfileId);
                }
                catch (Exception)
                {
                    TempData["errormessage"] = "Thay đổi trạng thái cho phép tìm kiếm thất bại!";
                    return RedirectToAction("List");
                }

                Profile profile = profileUnitOfWork.ProfileRepository.GetByID(profileIDNum);

                if (profile != null)
                {
                    if ("true".Equals(activeStatus))
                    {
                        profile.IsActive = true;
                    }
                    else
                    {
                        profile.IsActive = false;
                    }
                    profileUnitOfWork.ProfileRepository.Update(profile);
                    profileUnitOfWork.Save();

                    return RedirectToAction("List");
                }

                TempData["errormessage"] = "Thay đổi trạng thái cho phép tìm kiếm thất bại!";
                return RedirectToAction("List");
            }

            TempData["errormessage"] = "Thay đổi trạng thái cho phép tìm kiếm thất bại!";
            return RedirectToAction("List");
        }

        [Authorize(Roles = "Jobseeker")]
        [HttpPost]
        public ActionResult Delete(string profileID)
        {
            if (!String.IsNullOrEmpty(profileID))
            {
                int profileIDNum;

                try
                {
                    profileIDNum = Int32.Parse(profileID);
                }
                catch (Exception)
                {
                    TempData["errormessage"] = "Xóa hồ sơ thất bại!";
                    return RedirectToAction("List");
                }

                Profile profile = profileUnitOfWork.ProfileRepository.GetByID(profileIDNum);

                if (profile != null)
                {
                    profile.IsActive = false;
                    profile.IsDeleted = true;
                    profileUnitOfWork.ProfileRepository.Update(profile);

                    IEnumerable<ExpectedCity> expectedCities = profileUnitOfWork.ExpectedCityRepository.Get(s => s.ProfileID == profile.ProfileID).AsEnumerable();
                    if (expectedCities != null)
                    {
                        foreach (ExpectedCity item in expectedCities)
                        {
                            item.IsDeleted = true;
                            profileUnitOfWork.ExpectedCityRepository.Update(item);
                        }
                    }

                    IEnumerable<ExpectedCategory> expectedCategories = profileUnitOfWork.ExpectedCategoryRepository.Get(s => s.ProfileID == profile.ProfileID).AsEnumerable();
                    if (expectedCategories != null)
                    {
                        foreach (ExpectedCategory item in expectedCategories)
                        {
                            item.IsDeleted = false;
                            profileUnitOfWork.ExpectedCategoryRepository.Update(item);
                        }
                    }

                    profileUnitOfWork.Save();

                    TempData["successmessage"] = "Xóa hồ sơ thành công.";
                    return RedirectToAction("List");
                }

                TempData["errormessage"] = "Xóa hồ sơ thất bại!";
                return RedirectToAction("List");
            }

            TempData["errormessage"] = "Xóa hồ sơ thất bại!";
            return RedirectToAction("List");
        }
	}
}
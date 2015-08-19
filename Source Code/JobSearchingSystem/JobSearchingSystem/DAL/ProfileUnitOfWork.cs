using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using JobSearchingSystem.Models;
using System.Data;

namespace JobSearchingSystem.DAL
{
    public class ProfileUnitOfWork : UnitOfWork
    {
        public IEnumerable<ProListItem> GetAllProList(string jobseekerId)
        {
            if (!String.IsNullOrEmpty(jobseekerId)
                && this.JobseekerRepository.GetByID(jobseekerId) != null)
            {
                int no = 0;
                IEnumerable<ProListItem> outList = (from p in this.ProfileRepository.Get()
                                                    where p.IsDeleted == false
                                                          && p.JobSeekerID == jobseekerId
                                                    select new ProListItem
                                                    {
                                                        ProfileID = p.ProfileID,
                                                        No = ++no,
                                                        ProfileName = p.Name,
                                                        IsActive = p.IsActive,
                                                        ViewedCount = (from pr in this.ViewProfileRepository.Get()
                                                                       where pr.ProfileID == p.ProfileID
                                                                       select pr).Count(),
                                                        UpdatedTime = p.UpdatedTime,
                                                        PerccentStatus = p.PercentStatus,
                                                        IsDeleted = p.IsDeleted
                                                    }).AsEnumerable();

                return outList;
            }

            return null;
        }

        public bool UpdateContact(Contact contact)
        {
            Jobseeker jobseeker = this.JobseekerRepository.GetByID(contact.UserID);
            if (jobseeker == null)
            {
                return false;
            }
            if (jobseeker.IsDeleted == true)
            {
                return false;
            }

            try
            {
                if (this.ContactRepository.GetByID(jobseeker.JobSeekerID) == null)
                {
                    this.ContactRepository.Insert(contact);
                    this.Save();
                }
                else
                {
                    Contact contactToUpdate = this.ContactRepository.GetByID(jobseeker.JobSeekerID);
                    contactToUpdate.FullName = contact.FullName;
                    contactToUpdate.Gender = contact.Gender;
                    contactToUpdate.MaritalStatus = contact.MaritalStatus;
                    contactToUpdate.Nationality = contact.Nationality;
                    contactToUpdate.Address = contact.Address;
                    contactToUpdate.DateofBirth = contact.DateofBirth;
                    contactToUpdate.PhoneNumber = contact.PhoneNumber;
                    contactToUpdate.CityID = contact.CityID;
                    contactToUpdate.District = contact.District;
                    contactToUpdate.IsVisible = contact.IsVisible;
                    this.ContactRepository.Update(contactToUpdate);
                    this.Save();
                }
            }
            catch (DataException)
            {
                return false;
            }

            return true;
        }

        public bool UpdateCommonInfo(Profile profile, int expectedCityNum, int categoryID)
        {
            Jobseeker jobseeker = this.JobseekerRepository.GetByID(profile.JobSeekerID);
            if (jobseeker == null)
            {
                return false;
            }
            if (jobseeker.IsDeleted == true)
            {
                return false;
            }

            try
            {
                if (this.ProfileRepository.Get(filter: d => d.Name == profile.Name).FirstOrDefault() == null)
                {
                    // Add new
                    this.ProfileRepository.Insert(profile);
                    this.Save();

                    Profile insertedProfile = this.ProfileRepository.Get(filter: d => d.Name == profile.Name).LastOrDefault();
                    if (insertedProfile == null)
                    {
                        return false;
                    }

                    City city = this.CityRepository.GetByID(expectedCityNum);
                    if (city != null)
                    {
                        ExpectedCity expectedCity = new ExpectedCity();
                        expectedCity.ProfileID = insertedProfile.ProfileID;
                        expectedCity.CityID = city.CityID;
                        expectedCity.IsDeleted = false;

                        this.ExpectedCityRepository.Insert(expectedCity);
                        this.Save();
                    }

                    Category category = this.CategoryRepository.GetByID(categoryID);
                    if (category != null)
                    {
                        ExpectedCategory expectedCategory = new ExpectedCategory();
                        expectedCategory.ProfileID = insertedProfile.ProfileID;
                        expectedCategory.CategoryID = category.CategoryID;
                        expectedCategory.IsDeleted = false;

                        this.ExpectedCategoryRepository.Insert(expectedCategory);
                        this.Save();
                    }
                }
                else
                {
                    // Update
                    Profile profileToUpdate = this.ProfileRepository.Get(filter: d => d.Name == profile.Name).FirstOrDefault();
                    profileToUpdate.YearOfExperience = profile.YearOfExperience;
                    profileToUpdate.HighestSchoolLevel_ID = profile.HighestSchoolLevel_ID;
                    profileToUpdate.LanguageID = profile.LanguageID;
                    profileToUpdate.Level_ID = profile.Level_ID;
                    profileToUpdate.MostRecentCompany = profile.MostRecentCompany;
                    profileToUpdate.MostRecentPosition = profile.MostRecentPosition;
                    profileToUpdate.CurrentJobLevel_ID = profile.CurrentJobLevel_ID;
                    profileToUpdate.ExpectedPosition = profile.ExpectedPosition;
                    profileToUpdate.ExpectedJobLevel_ID = profile.ExpectedJobLevel_ID;
                    profileToUpdate.ExpectedSalary = profile.ExpectedSalary;
                    profileToUpdate.UpdatedTime = DateTime.Now;
                    profileToUpdate.Objectives = profile.Objectives;

                    this.ProfileRepository.Update(profileToUpdate);
                    this.Save();
                    
                    IEnumerable<ExpectedCity> oldExpectedCities = this.ExpectedCityRepository.Get(filter: d => d.ProfileID == profileToUpdate.ProfileID).AsEnumerable();
                    foreach (ExpectedCity item in oldExpectedCities)
                    {
                        if (item.IsDeleted == false)
                        {
                            item.IsDeleted = true;
                            this.ExpectedCityRepository.Update(item);
                            this.Save();
                        }
                    }
                    ExpectedCity expectedCity = this.ExpectedCityRepository.Get(s => s.ProfileID == profileToUpdate.ProfileID && s.CityID == expectedCityNum).FirstOrDefault();
                    if (expectedCity != null)
                    {
                        expectedCity.IsDeleted = false;
                        this.ExpectedCityRepository.Update(expectedCity);
                        this.Save();
                    }
                    else
                    {
                        City city = this.CityRepository.GetByID(expectedCityNum);
                        if (city != null)
                        {
                            ExpectedCity newExpectedCity = new ExpectedCity();
                            newExpectedCity.ProfileID = profileToUpdate.ProfileID;
                            newExpectedCity.CityID = city.CityID;
                            newExpectedCity.IsDeleted = false;

                            this.ExpectedCityRepository.Insert(newExpectedCity);
                            this.Save();
                        }
                    }

                    IEnumerable<ExpectedCategory> oldExpectedCategories = this.ExpectedCategoryRepository.Get(filter: d => d.ProfileID == profileToUpdate.ProfileID).AsEnumerable();
                    foreach (ExpectedCategory item in oldExpectedCategories)
                    {
                        if (item.IsDeleted == false)
                        {
                            item.IsDeleted = true;
                            this.ExpectedCategoryRepository.Update(item);
                            this.Save();
                        }
                    }
                    ExpectedCategory expectedCategory = this.ExpectedCategoryRepository.Get(s => s.ProfileID == profileToUpdate.ProfileID && s.CategoryID == categoryID).FirstOrDefault();
                    if (expectedCategory != null)
                    {
                        expectedCategory.IsDeleted = false;
                        this.ExpectedCategoryRepository.Update(expectedCategory);
                        this.Save();
                    }
                    else
                    {
                        Category category = this.CategoryRepository.GetByID(categoryID);
                        if (category != null)
                        {
                            ExpectedCategory newExpectedCategory = new ExpectedCategory();
                            newExpectedCategory.ProfileID = profileToUpdate.ProfileID;
                            newExpectedCategory.CategoryID = category.CategoryID;
                            newExpectedCategory.IsDeleted = false;

                            this.ExpectedCategoryRepository.Insert(newExpectedCategory);
                            this.Save();
                        }
                    }
                }
            }
            catch (DataException)
            {
                return false;
            }

            return true;
        }
    }
}
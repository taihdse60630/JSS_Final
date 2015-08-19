using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JobSearchingSystem.DAL
{
    public class CommonListUnitOfWork : UnitOfWork
    {
        public bool CreateCity(string name)
        {
            if (!String.IsNullOrEmpty(name)){
                if (name.Length > 50)
                {
                    return false;
                }

                City city = this.CityRepository.Get(s => s.Name == name).FirstOrDefault();

                if (city == null)
                {
                    City newCity = new City();
                    newCity.Name = name;
                    newCity.IsDeleted = false;
                    this.CityRepository.Insert(newCity);
                    this.Save();

                    return true;
                }
                else if (city.IsDeleted == true)
                {
                    city.IsDeleted = false;
                    this.CityRepository.Update(city);
                    this.Save();

                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }

        public bool UpdateCity(string name, int id)
        {
            if (!String.IsNullOrEmpty(name))
            {
                City city = this.CityRepository.Get(s => s.CityID == id).FirstOrDefault();

                if (city != null)
                {
                    city.Name = name;
                    this.CityRepository.Update(city);
                    this.Save();
                    return true;
                }
                return false;
            }
            return false;
        }


        public bool DeleteCity(int id)
        {

                City city = this.CityRepository.Get(s => s.CityID == id).FirstOrDefault();

                if (city != null)
                {
                    city.IsDeleted = true;
                    this.CityRepository.Update(city);
                    this.Save();
                    return true;
                }
                return false;
            
        }

        public IEnumerable<Category> GetCategoryList()
        {
            bool isDeleted = false;
            var categoryList = (from c in this.CategoryRepository.Get()
                            where c.IsDeleted == isDeleted
                            select c).AsEnumerable();
            return categoryList;
        }

        public bool CreateCategory(string name, string description)
        {
            if (!String.IsNullOrEmpty(name))
            {
                Category category = this.CategoryRepository.Get(s => s.Name == name).FirstOrDefault();

                if (category == null)
                {
                    Category newCategory = new Category();
                    newCategory.Name = name;
                    newCategory.Description = description;
                    newCategory.IsDeleted = false;
                    this.CategoryRepository.Insert(newCategory);
                    this.Save();
                    return true;
                }
                else if (category.IsDeleted == true)
                {
                    category.IsDeleted = false;
                    this.CategoryRepository.Update(category);
                    this.Save();

                    return true;
                }
                return false;
            }
            return false;
        }
        
        public bool UpdateCategory(string name, string description, int id)
        {
            if (!String.IsNullOrEmpty(name))
            {
                Category category = this.CategoryRepository.Get(s => s.CategoryID == id).FirstOrDefault();

                if (category != null)
                {
                    category.Name = name;
                    category.Description = description;
                    this.CategoryRepository.Update(category);
                    this.Save();
                    return true;
                }
                return false;
            }
            return false;
        }

        public bool DeleteCategory(int id)
        {
           
                Category category = this.CategoryRepository.Get(s => s.CategoryID == id).FirstOrDefault();

                if (category != null)
                {
                    category.IsDeleted = true;
                    this.CategoryRepository.Update(category);
                    this.Save();
                    return true;
                }
                return false;
        }

        public IEnumerable<Language> GetLanguageList()
        {
            bool isDeleted = false;
            var languageList = (from c in this.LanguageRepository.Get()
                                where c.IsDeleted == isDeleted
                                select c).AsEnumerable();
            return languageList;
        }

        public bool CreateLanguage(string name)
        {
            if (!String.IsNullOrEmpty(name))
            {
                Language language = this.LanguageRepository.Get(s => s.Name == name).FirstOrDefault();

                if (language == null)
                {
                    Language newLanguage = new Language();
                    newLanguage.Name = name;
                    newLanguage.IsDeleted = false;
                    this.LanguageRepository.Insert(newLanguage);
                    this.Save();
                    return true;
                }
                else if (language.IsDeleted == true)
                {
                    language.IsDeleted = false;
                    this.LanguageRepository.Update(language);
                    this.Save();

                    return true;
                }
                return false;
            }
            return false;
        }

        public bool UpdateLanguage(string name, int id)
        {
            if (!String.IsNullOrEmpty(name))
            {
                Language language = this.LanguageRepository.Get(s => s.LanguageID == id).FirstOrDefault();

                if (language != null)
                {
                    language.Name = name;
                    this.LanguageRepository.Update(language);
                    this.Save();
                    return true;
                }
                return false;
            }
            return false;
        }

        public bool DeleteLanguage(int id)
        {

            Language language = this.LanguageRepository.Get(s => s.LanguageID == id).FirstOrDefault();

            if (language != null)
            {
                language.IsDeleted = true;
                this.LanguageRepository.Update(language);
                this.Save();
                return true;
            }
            return false;
        }

        public IEnumerable<JobLevel> GetJobLevelList()
        {
            bool isDeleted = false;
            var jobLevelList = (from c in this.JobLevelRepository.Get()
                                where c.IsDeleted == isDeleted
                                select c).AsEnumerable();
            return jobLevelList;
        }

        public bool CreateJobLevel(string name, int levelNum)
        {
            if (!String.IsNullOrEmpty(name))
            {
                JobLevel jobLevel = this.JobLevelRepository.Get(s => s.Name == name).FirstOrDefault();

                if (jobLevel == null)
                {
                    JobLevel newJobLevel = new JobLevel();
                    newJobLevel.Name = name;
                    newJobLevel.LevelNum = levelNum;
                    newJobLevel.IsDeleted = false;
                    this.JobLevelRepository.Insert(newJobLevel);
                    this.Save();
                    return true;
                }
                else if (jobLevel.IsDeleted == true)
                {
                    jobLevel.IsDeleted = false;
                    jobLevel.LevelNum = levelNum;
                    this.JobLevelRepository.Update(jobLevel);
                    this.Save();

                    return true;
                }
                return false;
            }
            return false;
        }

        public bool UpdateJobLevel(string name, int levelNum, int id)
        {
            if (!String.IsNullOrEmpty(name))
            {
                JobLevel jobLevel = this.JobLevelRepository.Get(s => s.JobLevel_ID == id).FirstOrDefault();

                if (jobLevel != null)
                {
                    jobLevel.Name = name;
                    jobLevel.LevelNum = levelNum;
                    this.JobLevelRepository.Update(jobLevel);
                    this.Save();
                    return true;
                }
                return false;
            }
            return false;
        }

        public bool DeleteJobLevel(int id)
        {

            JobLevel jobLevel = this.JobLevelRepository.Get(s => s.JobLevel_ID == id).FirstOrDefault();

            if (jobLevel != null)
            {
                jobLevel.IsDeleted = true;
                this.JobLevelRepository.Update(jobLevel);
                this.Save();
                return true;
            }
            return false;
        }

        public IEnumerable<SchoolLevel> GetSchoolLevelList()
        {
            bool isDeleted = false;
            var schoolLevelList = (from c in this.SchoolLevelRepository.Get()
                                where c.IsDeleted == isDeleted
                                select c).AsEnumerable();
            return schoolLevelList;
        }

        public bool CreateSchoolLevel(string name, int levelNum)
        {
            if (!String.IsNullOrEmpty(name))
            {
                SchoolLevel schoolLevel = this.SchoolLevelRepository.Get(s => s.Name == name).FirstOrDefault();

                if (schoolLevel == null)
                {
                    SchoolLevel newSchoolLevel = new SchoolLevel();
                    newSchoolLevel.Name = name;
                    newSchoolLevel.LevelNum = levelNum;
                    newSchoolLevel.IsDeleted = false;
                    this.SchoolLevelRepository.Insert(newSchoolLevel);
                    this.Save();
                    return true;
                }
                else if (schoolLevel.IsDeleted == true)
                {
                    schoolLevel.IsDeleted = false;
                    schoolLevel.LevelNum = levelNum;
                    this.SchoolLevelRepository.Update(schoolLevel);
                    this.Save();

                    return true;
                }
                return false;
            }
            return false;
        }

        public bool UpdateSchoolLevel(string name, int levelNum, int id)
        {
            if (!String.IsNullOrEmpty(name))
            {
                SchoolLevel schoolLevel = this.SchoolLevelRepository.Get(s => s.SchoolLevel_ID == id).FirstOrDefault();

                if (schoolLevel != null)
                {
                    schoolLevel.Name = name;
                    schoolLevel.LevelNum = levelNum;
                    this.SchoolLevelRepository.Update(schoolLevel);
                    this.Save();
                    return true;
                }
                return false;
            }
            return false;
        }

        public bool DeleteSchoolLevel(int id)
        {

            SchoolLevel schoolLevel = this.SchoolLevelRepository.Get(s => s.SchoolLevel_ID == id).FirstOrDefault();

            if (schoolLevel != null)
            {
                schoolLevel.IsDeleted = true;
                this.SchoolLevelRepository.Update(schoolLevel);
                this.Save();
                return true;
            }
            return false;
        }

        public IEnumerable<Level> GetLevelList()
        {
            bool isDeleted = false;
            var LevelList = (from c in this.LevelRepository.Get()
                                   where c.IsDeleted == isDeleted
                                   select c).AsEnumerable();
            return LevelList;
        }

        public bool CreateLevel(string name, int levelNum)
        {
            if (!String.IsNullOrEmpty(name))
            {
                Level level = this.LevelRepository.Get(s => s.Name == name).FirstOrDefault();

                if (level == null)
                {
                    Level newLevel = new Level();
                    newLevel.Name = name;
                    newLevel.LevelNum = levelNum;
                    newLevel.IsDeleted = false;
                    this.LevelRepository.Insert(newLevel);
                    this.Save();
                    return true;
                }
                else if (level.IsDeleted == true)
                {
                    level.IsDeleted = false;
                    level.LevelNum = levelNum;
                    this.LevelRepository.Update(level);
                    this.Save();

                    return true;
                }
                return false;
            }
            return false;
        }

        public bool UpdateLevel(string name, int levelNum, int id)
        {
            if (!String.IsNullOrEmpty(name))
            {
                Level Level = this.LevelRepository.Get(s => s.Level_ID == id).FirstOrDefault();

                if (Level != null)
                {
                    Level.Name = name;
                    Level.LevelNum = levelNum;
                    this.LevelRepository.Update(Level);
                    this.Save();
                    return true;
                }
                return false;
            }
            return false;
        }

        public bool DeleteLevel(int id)
        {

            Level Level = this.LevelRepository.Get(s => s.Level_ID == id).FirstOrDefault();

            if (Level != null)
            {
                Level.IsDeleted = true;
                this.LevelRepository.Update(Level);
                this.Save();
                return true;
            }
            return false;
        }

    }      

}
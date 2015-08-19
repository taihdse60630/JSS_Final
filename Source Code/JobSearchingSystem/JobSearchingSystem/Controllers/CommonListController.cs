using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using JobSearchingSystem;
using JobSearchingSystem.DAL;
using JobSearchingSystem.Models;

namespace JobSearchingSystem.Controllers
{
    public class CommonListController : Controller
    {
        private CommonListUnitOfWork commonListUnitOfWork = new CommonListUnitOfWork();

        //
        // GET: /CommonList/
        public ActionResult Index()
        {
            ComIndexViewModel model = new ComIndexViewModel();
            model.cities = commonListUnitOfWork.CityRepository.Get(s => s.IsDeleted == false).OrderBy(s => s.Name).AsEnumerable();
            model.categories = commonListUnitOfWork.CategoryRepository.Get(s => s.IsDeleted == false).OrderBy(s => s.Name).AsEnumerable();
            model.languages = commonListUnitOfWork.LanguageRepository.Get(s => s.IsDeleted == false).OrderBy(s => s.Name).AsEnumerable();
            model.jobLevels = commonListUnitOfWork.JobLevelRepository.Get(s => s.IsDeleted == false).OrderByDescending(s => s.LevelNum).AsEnumerable();
            model.schoolLevels = commonListUnitOfWork.SchoolLevelRepository.Get(s => s.IsDeleted == false).OrderByDescending(s => s.LevelNum).AsEnumerable();
            model.levels = commonListUnitOfWork.LevelRepository.Get(s => s.IsDeleted == false).OrderByDescending(s => s.LevelNum).AsEnumerable();

            return View(model);
        }

        public ActionResult CityList()
        {
            ComCityListViewModel model = new ComCityListViewModel();
            model.cities = commonListUnitOfWork.CityRepository.Get(s => s.IsDeleted == false).OrderBy(s => s.Name).AsEnumerable();
            model.name = "";

            return View(model);
        }

        [HttpPost]
        public ActionResult CreateCity(string name)
        {
            if (ModelState.IsValid)
            {
                if (!String.IsNullOrEmpty(name))
                {
                    bool result = commonListUnitOfWork.CreateCity(name);

                    return RedirectToAction("CityList");
                }
            }

            return RedirectToAction("CityList");
        }

        [HttpPost]
        public ActionResult UpdateCity(string id, string name)
        {
           
                if (!String.IsNullOrEmpty(id) && !String.IsNullOrEmpty(name))
                {
                    bool result = commonListUnitOfWork.UpdateCity(name, Int32.Parse(id));
                    ComCityListViewModel model = new ComCityListViewModel();
                    
                    return RedirectToAction("CityList", model);
                }
            
            return RedirectToAction("CityList");
        }

        [HttpPost]
        public ActionResult DeleteCity(int id)
        {

            bool result = commonListUnitOfWork.DeleteCity(id);
            ComCityListViewModel model = new ComCityListViewModel();

            return RedirectToAction("CityList", model);
        }

        public ActionResult CategoryList()
        {
            ComCategoryListViewModel model = new ComCategoryListViewModel();
            model.categories = commonListUnitOfWork.CategoryRepository.Get(s => s.IsDeleted == false).OrderBy(s => s.Name).AsEnumerable();
            model.name = "";

            return View(model);
        }        

        [HttpPost]
        public ActionResult CreateCategory(ComCategoryListViewModel model)
        {
            if (ModelState.IsValid && model != null)
            {
                if (!String.IsNullOrEmpty(model.name))
                {
                    bool result = commonListUnitOfWork.CreateCategory(model.name, model.description);

                    if (result)
                    {
                        model.message = "Tạo nhóm ngành nghề thành công.";
                    }
                    else
                    {
                        model.message = "Tạo nhóm ngành nghề thất bại.";
                    }

                    return RedirectToAction("CategoryList");
                }

                return RedirectToAction("CategoryList");
            }
            return RedirectToAction("CategoryList");
        }

        [HttpPost]
        public ActionResult UpdateCategory(int id, string name, string description)
        {
            
            if (!String.IsNullOrEmpty(name))
                {
                    bool result = commonListUnitOfWork.UpdateCategory(name, description, id);
                    ComCategoryListViewModel model = new ComCategoryListViewModel();
                    if (result)
                    {
                        model.message = "Cập nhật nhóm ngành nghề thành công.";
                    }
                    else
                    {
                        model.message = "Cập nhật nhóm ngành nghề thất bại.";
                    }

                    return RedirectToAction("CategoryList");
            }
            return RedirectToAction("CategoryList");
        }
        
        [HttpPost]
        public ActionResult DeleteCategory(int id)
        {
           
                bool result = commonListUnitOfWork.DeleteCategory(id);
                ComCategoryListViewModel model = new ComCategoryListViewModel();
                if (result)
                {
                    model.message = "Xóa nhóm ngành nghề thành công.";
                }
                else
                {
                    model.message = "Xóa nhóm ngành nghề thất bại.";
                }

                return RedirectToAction("CategoryList");
           
        }

        public ActionResult LanguageList()
        {
            ComLanguageViewModel model = new ComLanguageViewModel();
            model.languages = commonListUnitOfWork.LanguageRepository.Get(s => s.IsDeleted == false).OrderBy(s => s.Name).AsEnumerable();
            model.name = "";

            return View(model);
        }        

        [HttpPost]
        public ActionResult CreateLanguage(ComLanguageViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (!String.IsNullOrEmpty(model.name))
                {
                    bool result = commonListUnitOfWork.CreateLanguage(model.name);

                    if (result)
                    {
                        model.message = "Tạo ngôn ngữ thành công.";
                    }
                    else
                    {
                        model.message = "Tạo ngôn ngữ thất bại.";
                    }

                    return RedirectToAction("LanguageList");
                }

                return RedirectToAction("LanguageList");
            }
            return RedirectToAction("LanguageList");
        }

        [HttpPost]
        public ActionResult UpdateLanguage(int id, string name)
        {

            if (!String.IsNullOrEmpty(name))
            {
                bool result = commonListUnitOfWork.UpdateLanguage(name, id);
                ComLanguageViewModel model = new ComLanguageViewModel();
                if (result)
                {
                    model.message = "Cập nhật ngôn ngữ thành công.";
                }
                else
                {
                    model.message = "Cập nhật ngôn ngữ thất bại.";
                }

                return RedirectToAction("LanguageList");
            }
            return RedirectToAction("LanguageList");
        }

        [HttpPost]
        public ActionResult DeleteLanguage(int id)
        {

            bool result = commonListUnitOfWork.DeleteLanguage(id);
            ComLanguageViewModel model = new ComLanguageViewModel();
            if (result)
            {
                model.message = "Xóa ngôn ngữ thành công.";
            }
            else
            {
                model.message = "Xóa ngôn ngữ thất bại.";
            }

            return RedirectToAction("LanguageList");

        }

        public ActionResult JobLevelList()
        {

            ComJobLevelViewModel model = new ComJobLevelViewModel();
            model.jobLevels = commonListUnitOfWork.JobLevelRepository.Get(s => s.IsDeleted == false).OrderByDescending(s => s.LevelNum).AsEnumerable();
            model.name = "";

            return View(model);
        }

        [HttpPost]
        public ActionResult CreateJobLevel(ComJobLevelViewModel model)
        {
            if (model != null)
            {
                if (!String.IsNullOrEmpty(model.name))
                {
                    bool result = commonListUnitOfWork.CreateJobLevel(model.name, model.levelNum);

                    if (result)
                    {
                        model.message = "Tạo JobLevel thành công.";
                    }
                    else
                    {
                        model.message = "Tạo JobLevel thất bại.";
                    }

                    return RedirectToAction("JobLevelList");
                }

                return RedirectToAction("JobLevelList");
            }
            return RedirectToAction("JobLevelList");
        }

        [HttpPost]
        public ActionResult UpdateJobLevel(int id, string name, int levelNum)
        {

            if (!String.IsNullOrEmpty(name))
            {
                bool result = commonListUnitOfWork.UpdateJobLevel(name, levelNum, id);
                ComJobLevelViewModel model = new ComJobLevelViewModel();
                if (result)
                {
                    model.message = "Cập nhật JobLevel thành công.";
                }
                else
                {
                    model.message = "Cập nhật JobLevel thất bại.";
                }

                return RedirectToAction("JobLevelList");
            }
            return RedirectToAction("JobLevelList");
        }

        [HttpPost]
        public ActionResult DeleteJobLevel(int id)
        {

            bool result = commonListUnitOfWork.DeleteJobLevel(id);
            ComJobLevelViewModel model = new ComJobLevelViewModel();
            if (result)
            {
                model.message = "Xóa JobLevel thành công.";
            }
            else
            {
                model.message = "Xóa JobLevel thất bại.";
            }

            return RedirectToAction("JobLevelList");

        }

        public ActionResult SchoolLevelList()
        {

            ComSchoolLevelViewModel model = new ComSchoolLevelViewModel();
            model.schoolLevels = commonListUnitOfWork.SchoolLevelRepository.Get(s => s.IsDeleted == false).OrderByDescending(s => s.LevelNum).AsEnumerable();
            model.name = "";

            return View(model);
        }

        [HttpPost]
        public ActionResult CreateSchoolLevel(ComSchoolLevelViewModel model)
        {
            if (model != null)
            {
                if (!String.IsNullOrEmpty(model.name))
                {
                    bool result = commonListUnitOfWork.CreateSchoolLevel(model.name, model.levelNum);

                    if (result)
                    {
                        model.message = "Tạo SchoolLevel thành công.";
                    }
                    else
                    {
                        model.message = "Tạo SchoolLevel thất bại.";
                    }

                    return RedirectToAction("SchoolLevelList");
                }

                return RedirectToAction("SchoolLevelList");
            }
            return RedirectToAction("SchoolLevelList");
        }

        [HttpPost]
        public ActionResult UpdateSchoolLevel(int id, string name, int levelNum)
        {

            if (!String.IsNullOrEmpty(name))
            {
                bool result = commonListUnitOfWork.UpdateSchoolLevel(name, levelNum, id);
                ComSchoolLevelViewModel model = new ComSchoolLevelViewModel();
                if (result)
                {
                    model.message = "Cập nhật SchoolLevel thành công.";
                }
                else
                {
                    model.message = "Cập nhật SchoolLevel thất bại.";
                }

                return RedirectToAction("SchoolLevelList");
            }
            return RedirectToAction("SchoolLevelList");
        }

        [HttpPost]
        public ActionResult DeleteSchoolLevel(int id)
        {

            bool result = commonListUnitOfWork.DeleteSchoolLevel(id);
            ComSchoolLevelViewModel model = new ComSchoolLevelViewModel();
            if (result)
            {
                model.message = "Xóa SchoolLevel thành công.";
            }
            else
            {
                model.message = "Xóa SchoolLevel thất bại.";
            }

            return RedirectToAction("SchoolLevelList");

        }

        public ActionResult LevelList()
        {

            ComLevelViewModel model = new ComLevelViewModel();
            model.levels = commonListUnitOfWork.LevelRepository.Get(s => s.IsDeleted == false).OrderByDescending(s => s.LevelNum).AsEnumerable();
            model.name = "";

            return View(model);
        }

        [HttpPost]
        public ActionResult CreateLevel(ComLevelViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (!String.IsNullOrEmpty(model.name))
                {
                    bool result = commonListUnitOfWork.CreateLevel(model.name, model.levelNum);

                    if (result)
                    {
                        model.message = "Tạo Level thành công.";
                    }
                    else
                    {
                        model.message = "Tạo Level thất bại.";
                    }

                    return RedirectToAction("LevelList");
                }

                return RedirectToAction("LevelList");
            }
            return RedirectToAction("LevelList");
        }

        [HttpPost]
        public ActionResult UpdateLevel(int id, string name, int levelNum)
        {

            if (!String.IsNullOrEmpty(name))
            {
                bool result = commonListUnitOfWork.UpdateLevel(name, levelNum, id);
                ComLevelViewModel model = new ComLevelViewModel();
                if (result)
                {
                    model.message = "Cập nhật Level thành công.";
                }
                else
                {
                    model.message = "Cập nhật Level thất bại.";
                }

                return RedirectToAction("LevelList");
            }
            return RedirectToAction("LevelList");
        }

        [HttpPost]
        public ActionResult DeleteLevel(int id)
        {

            bool result = commonListUnitOfWork.DeleteLevel(id);
            ComLevelViewModel model = new ComLevelViewModel();
            if (result)
            {
                model.message = "Xóa Level thành công.";
            }
            else
            {
                model.message = "Xóa Level thất bại.";
            }

            return RedirectToAction("LevelList");

        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                commonListUnitOfWork.Dispose();
            }
            base.Dispose(disposing);
        }

	}
}
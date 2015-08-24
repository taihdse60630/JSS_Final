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
    [Authorize(Roles = "Staff")]
    [MessageFilter]
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

                    if (result)
                    {
                        TempData["successmessage"] = "Tạo thành phố " + name + " thành công.";
                    }
                    else
                    {
                        TempData["errormessage"] = "Tạo thành phố " + name + " thất bại!";
                    }
                }
                else
                {
                    TempData["errormessage"] = "Dữ liệu không hợp lệ!";
                }
            }
            else
            {
                TempData["errormessage"] = "Có lỗi xảy ra!";
            }
            
            return RedirectToAction("CityList");
        }

        [HttpPost]
        public ActionResult UpdateCity(string id, string name)
        {
            if (!String.IsNullOrEmpty(id) && !String.IsNullOrEmpty(name))
            {
                bool result = commonListUnitOfWork.UpdateCity(name, Int32.Parse(id));

                if (result)
                {
                    TempData["successmessage"] = "Cập nhật thành phố " + name + " thành công.";
                }
                else
                {
                    TempData["errormessage"] = "Cập nhật thành phố " + name + " thất bại!";
                }
            }
            else
            {
                TempData["errormessage"] = "Dữ liệu không hợp lệ!";
            }
            
            return RedirectToAction("CityList");
        }

        [HttpPost]
        public ActionResult DeleteCity(int id)
        {
            bool result = commonListUnitOfWork.DeleteCity(id);

            if (result)
            {
                TempData["successmessage"] = "Xóa thành phố thành công.";
            }
            else
            {
                TempData["errormessage"] = "Xóa thành phố thất bại!";
            }

            return RedirectToAction("CityList");
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
                        TempData["successmessage"] = "Tạo nhóm ngành nghề " + model.name + " thành công.";
                    }
                    else
                    {
                        TempData["errormessage"] = "Tạo nhóm ngành nghề " + model.name + " thất bại!";
                    }
                }
                else
                {
                    TempData["errormessage"] = "Dữ liệu không hợp lệ!";
                }
            }
            else
            {
                TempData["errormessage"] = "Có lỗi xảy ra!";
            }

            return RedirectToAction("CategoryList");
        }

        [HttpPost]
        public ActionResult UpdateCategory(int id, string name, string description)
        {
            
            if (!String.IsNullOrEmpty(name))
            {
                bool result = commonListUnitOfWork.UpdateCategory(name, description, id);
                    
                if (result)
                {
                    TempData["successmessage"] = "Cập nhật nhóm ngành nghề " + name + " thành công.";
                }
                else
                {
                    TempData["errormessage"] = "Cập nhật nhóm ngành nghề " + name + " thất bại!";
                }
            }
            else
            {
                TempData["errormessage"] = "Dữ liệu không hợp lệ!";
            }

            return RedirectToAction("CategoryList");
        }
        
        [HttpPost]
        public ActionResult DeleteCategory(int id)
        {
           
            bool result = commonListUnitOfWork.DeleteCategory(id);

            if (result)
            {
                TempData["successmessage"] = "Xóa nhóm ngành nghề thành công.";
            }
            else
            {
                TempData["errormessage"] = "Xóa nhóm ngành nghề thất bại!";
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
                        TempData["successmessage"] = "Tạo ngôn ngữ " + model.name + " thành công.";
                    }
                    else
                    {
                        TempData["errormessage"] = "Tạo ngôn ngữ " + model.name + " thất bại!";
                    }
                }
                else
                {
                    TempData["errormessage"] = "Dữ liệu không hợp lệ!";
                }
            }
            else
            {
                TempData["errormessage"] = "Có lỗi xảy ra!";
            }

            return RedirectToAction("LanguageList");
        }

        [HttpPost]
        public ActionResult UpdateLanguage(int id, string name)
        {

            if (!String.IsNullOrEmpty(name))
            {
                bool result = commonListUnitOfWork.UpdateLanguage(name, id);
                
                if (result)
                {
                    TempData["successmessage"] = "Cập nhật ngôn ngữ " + name + " thành công.";
                }
                else
                {
                    TempData["errormessage"] = "Cập nhật ngôn ngữ " + name + " thất bại!";
                }
            }
            else
            {
                TempData["errormessage"] = "Dữ liệu không hợp lệ!";
            }

            return RedirectToAction("LanguageList");
        }

        [HttpPost]
        public ActionResult DeleteLanguage(int id)
        {

            bool result = commonListUnitOfWork.DeleteLanguage(id);
            
            if (result)
            {
                TempData["successmessage"] = "Xóa ngôn ngữ thành công.";
            }
            else
            {
                TempData["errormessage"] = "Xóa ngôn ngữ thất bại!";
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
                        TempData["successmessage"] = "Tạo cấp bậc " + model.name + " thành công.";
                    }
                    else
                    {
                        TempData["errormessage"] = "Tạo cấp bậc " + model.name + " thất bại!";
                    }
                }
                else
                {
                    TempData["errormessage"] = "Dữ liệu không hợp lệ!";
                }
            }
            else
            {
                TempData["errormessage"] = "Có lỗi xảy ra!";
            }

            return RedirectToAction("JobLevelList");
        }

        [HttpPost]
        public ActionResult UpdateJobLevel(int id, string name, int levelNum)
        {

            if (!String.IsNullOrEmpty(name))
            {
                bool result = commonListUnitOfWork.UpdateJobLevel(name, levelNum, id);
                
                if (result)
                {
                    TempData["successmessage"] = "Cập nhật cấp bậc " + name + " thành công.";
                }
                else
                {
                    TempData["errormessage"] = "Cập nhật cấp bậc " + name + " thất bại!";
                }
            }
            else
            {
                TempData["errormessage"] = "Có lỗi xảy ra!";
            }

            return RedirectToAction("JobLevelList");
        }

        [HttpPost]
        public ActionResult DeleteJobLevel(int id)
        {

            bool result = commonListUnitOfWork.DeleteJobLevel(id);
            
            if (result)
            {
                TempData["successmessage"] = "Xóa cấp bậc thành công.";
            }
            else
            {
                TempData["errormessage"] = "Xóa cấp bậc thất bại!";
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
                        TempData["successmessage"] = "Tạo bằng cấp " + model.name + " thành công.";
                    }
                    else
                    {
                        TempData["errormessage"] = "Tạo bằng cấp " + model.name + " thất bại!";
                    }
                }
                else
                {
                    TempData["errormessage"] = "Dữ liệu không hợp lệ!";
                }
            }
            else
            {
                TempData["errormessage"] = "Có lỗi xảy ra!";
            }

            return RedirectToAction("SchoolLevelList");
        }

        [HttpPost]
        public ActionResult UpdateSchoolLevel(int id, string name, int levelNum)
        {

            if (!String.IsNullOrEmpty(name))
            {
                bool result = commonListUnitOfWork.UpdateSchoolLevel(name, levelNum, id);
                
                if (result)
                {
                    TempData["successmessage"] = "Cập nhật bằng cấp " + name + " thành công.";
                }
                else
                {
                    TempData["errormessage"] = "Cập nhật bằng cấp " + name + " thất bại!";
                }
            }
            else
            {
                TempData["errormessage"] = "Có lỗi xảy ra!";
            }

            return RedirectToAction("SchoolLevelList");
        }

        [HttpPost]
        public ActionResult DeleteSchoolLevel(int id)
        {

            bool result = commonListUnitOfWork.DeleteSchoolLevel(id);
            
            if (result)
            {
                TempData["successmessage"] = "Xóa bằng cấp thành công.";
            }
            else
            {
                TempData["errormessage"] = "Xóa bằng cấp thất bại!";
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
                        TempData["successmessage"] = "Tạo trình độ ngôn ngữ " + model.name + " thành công.";
                    }
                    else
                    {
                        TempData["errormessage"] = "Tạo trình độ ngôn ngữ " + model.name + " thất bại!";
                    }
                }
                else
                {
                    TempData["errormessage"] = "Dữ liệu không hợp lệ!";
                }
            }
            else
            {
                TempData["errormessage"] = "Có lỗi xảy ra!";
            }

            return RedirectToAction("LevelList");
        }

        [HttpPost]
        public ActionResult UpdateLevel(int id, string name, int levelNum)
        {

            if (!String.IsNullOrEmpty(name))
            {
                bool result = commonListUnitOfWork.UpdateLevel(name, levelNum, id);
                
                if (result)
                {
                    TempData["successmessage"] = "Cập nhật trình độ ngôn ngữ " + name + " thành công.";
                }
                else
                {
                    TempData["errormessage"] = "Cập nhật trình độ ngôn ngữ " + name + " thất bại!";
                }
            }
            else
            {
                TempData["errormessage"] = "Có lỗi xảy ra!";
            }

            return RedirectToAction("LevelList");
        }

        [HttpPost]
        public ActionResult DeleteLevel(int id)
        {

            bool result = commonListUnitOfWork.DeleteLevel(id);
            
            if (result)
            {
                TempData["successmessage"] = "Xóa trình độ ngôn ngữ thành công.";
            }
            else
            {
                TempData["errormessage"] = "Xóa trình độ ngôn ngữ thất bại!";
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
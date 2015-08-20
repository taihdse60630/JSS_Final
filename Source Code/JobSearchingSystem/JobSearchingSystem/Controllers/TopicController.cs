using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using JobSearchingSystem;
using JobSearchingSystem.DAL;
using JobSearchingSystem.Models;
using System.Web.UI.WebControls;
using System.IO;

namespace JobSearchingSystem.Controllers
{
    public class TopicController : Controller
    {
        private TopicFunctionModel functionModel = new TopicFunctionModel();

        // GET: /Topic/
        public ActionResult Index()
        {
            return List();
        }

        public ActionResult List()
        {
            return View(functionModel.GetListTopic());
        }

        public ActionResult Create()
        {
            TopCreateViewModel model = new TopCreateViewModel();
            return View(model);
        }

        //Update topic
        public ActionResult Update(int topicID)
        {
            return View(functionModel.GetTopicByID(topicID));
        }
      
        [HttpPost]
        public ActionResult Update(TopCreateViewModel model, HttpPostedFileBase file)
        {
            //Get user ID here
            model.UpdateStaffID = "2c6c4ab1-feb7-49fc-a84e-7fbf9736e7fc";
            model.UpdatedDate = DateTime.Now;
            
            //File uploade code
            if (file != null && file.ContentLength > 0)
            {
                string path = "/Content/img/TopicImg";
                // extract only the fielname
                model.ImageUrl = Path.GetFileName(file.FileName);
                // store the file inside ~/App_Data/uploads folder
                model.ImageUrl = Path.Combine(Server.MapPath(path), model.ImageUrl);
                file.SaveAs(model.ImageUrl);
                model.ImageUrl = path + "/" + Path.GetFileName(file.FileName);
            }

            if (functionModel.UpdateTopic(model))
            {
                return RedirectToAction("OwnList");
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(TopCreateViewModel model, HttpPostedFileBase file)
        {
            //Get user ID here
            model.WriterID = "2c6c4ab1-feb7-49fc-a84e-7fbf9736e7fc";
            model.CreatedDate = DateTime.Now;

            //File uploade code
            if (file != null && file.ContentLength > 0)
            {
                string path = "/Content/img/TopicImg";
                // extract only the fielname
                model.ImageUrl = Path.GetFileName(file.FileName);
                // store the file inside ~/App_Data/uploads folder
                model.ImageUrl = Path.Combine(Server.MapPath(path), model.ImageUrl);
                file.SaveAs(model.ImageUrl);
                model.ImageUrl = path + "/" + Path.GetFileName(file.FileName);
            }

            if (functionModel.CreateTopic(model))
            {
                return RedirectToAction("OwnList");
            }
            return View(model);
        }

        public ActionResult Detail(int id = 0)
        {
            TopicItem item = functionModel.GetTopicDetailByID(id);
            if (item != null)
            {
                return View(item);
            }
            else
            {
                return List();
            }
        }

        //Display own list.
        public ActionResult OwnList()
        {
            string userID = "2c6c4ab1-feb7-49fc-a84e-7fbf9736e7fc"; //Code for getting user ID is putting here.
            TopListViewModel model = functionModel.GetListByUserID(userID);
            return View(model);
        }

        //Delete topic
        [HttpPost]
        public ActionResult Delete(int topicID)
        {
            if (functionModel.DeleteTopic(topicID) == true)
            {
                return RedirectToAction("OwnList");
            }
            else
            {
                return RedirectToAction("OwnList");
            }
        }

        //List all topics waiting for approve
        public ActionResult PendingList()
        {
            return View(functionModel.GetPendingList());
        }

        //Approve topic
        [HttpPost]
        public ActionResult Approve(int topicID)
        {
            if (functionModel.ApproveTopic(topicID) == true)
            {
                return RedirectToAction("PendingList");
            }
            else
            {
                return RedirectToAction("PendingList");
            }
        }

        //Disapprove topic
        [HttpPost]
        public ActionResult Disapprove(int topicID)
        {
            if (functionModel.DisapproveTopic(topicID) == true)
            {
                return RedirectToAction("PendingList");
            }
            else
            {
                return RedirectToAction("PendingList");
            }
        }
    }
}
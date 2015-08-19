using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using JobSearchingSystem;
using JobSearchingSystem.DAL;
using JobSearchingSystem.Models;

namespace JobSearchingSystem.Controllers
{
    public class TopicController : Controller
    {
        private TopicUnitOfWork topicUnitOfWork = new TopicUnitOfWork();
        
        // GET: /Topic/
        public ActionResult Index()
        {
            return List();
        }

        public ActionResult List()
        {
            TopListViewModel model = new TopListViewModel(topicUnitOfWork.Get());
            return View(model);
        }

        public ActionResult Create()
        {
            TopCreateViewModel model = new TopCreateViewModel();

            return View(model);
        }
        
        [HttpPost]
        public ActionResult Create(TopCreateViewModel model)
        {
            if (model != null)
            {
                Topic newTopic = new Topic();
                newTopic.WriterID = "f1be6249-eddf-4e78-9c13-9c3a0268f227";
                newTopic.Topic_content = model.Topic_content;
                newTopic.CreatedDate = DateTime.Now;
                newTopic.UpdatedDate = null;
                newTopic.UpdatedStaffID = null;
                newTopic.IsApproved = false;
                newTopic.IsDeleted = false;
                    
                bool result = this.topicUnitOfWork.Create(newTopic);

                if (result) 
                {
                    return RedirectToAction("List");
                }
                return View(model);
            }
            return View(model);
        }

        public ActionResult Detail(int id = 0)
        {
            TopicItem item = new TopicItem(topicUnitOfWork.GetByID(id));
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
            string userID = "f1be6249-eddf-4e78-9c13-9c3a0268f227"; //Code for getting user ID is putting here.
            TopListViewModel model = new TopListViewModel(topicUnitOfWork.GetByUserID(userID));
            return View(model);
        }

        //Delete topic
        [HttpPost]
        public ActionResult Delete(int topicID)
        {
            if (topicUnitOfWork.Delete(topicID) == true)
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
            return View(topicUnitOfWork.GetPendingList());
        }

        //Approve topic
        [HttpPost]
        public ActionResult Approve(int topicID)
        {
            if (topicUnitOfWork.Approve(topicID) == true)
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
            if (topicUnitOfWork.Disapprove(topicID) == true)
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
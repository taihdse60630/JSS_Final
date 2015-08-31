using JobSearchingSystem.DAL;
using JobSearchingSystem.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JobSearchingSystem.Controllers
{
    [MessageFilter]
    public class MessageController : Controller
    {
        
        private MessageUnitOfWork messageUnitOfWork = new MessageUnitOfWork();
        //
        // GET: /Message/
       
        public ActionResult Index()
        {
            return List();
        }

        [Authorize]
        public ActionResult List()
        {
            MessageViewModel model = new MessageViewModel();
            string receiverID = messageUnitOfWork.AspNetUserRepository.Get(s => s.UserName == User.Identity.Name).FirstOrDefault().Id;
            model.messageList = messageUnitOfWork.getAllMessage(receiverID);
            model.typeOfMessage = "allMessage";
            model.numberOfInbox = model.messageList.Count();
            model.numberOfSent = messageUnitOfWork.getAllSentMessage(receiverID).Count();
            model.numberOfDeleted = messageUnitOfWork.getAllDeleteMessage(receiverID).Count();
            return View(model);
        }

        public ActionResult SentMessageList()
        {
             MessageViewModel model = new MessageViewModel();
              string receiverID = messageUnitOfWork.AspNetUserRepository.Get(s => s.UserName == User.Identity.Name).FirstOrDefault().Id;
              model.messageList = messageUnitOfWork.getAllSentMessage(receiverID);
              model.typeOfMessage = "sentMessage";
              model.numberOfInbox = messageUnitOfWork.getAllMessage(receiverID).Count();
              model.numberOfSent = model.messageList.Count();
              model.numberOfDeleted = messageUnitOfWork.getAllDeleteMessage(receiverID).Count();
             return View("List", model);
        }

        public ActionResult DeletedMessageList()
        {
            MessageViewModel model = new MessageViewModel();
            string receiverID = messageUnitOfWork.AspNetUserRepository.Get(s => s.UserName == User.Identity.Name).FirstOrDefault().Id;
            model.messageList = messageUnitOfWork.getAllDeleteMessage(receiverID);
            model.typeOfMessage = "deletedMessage";

            model.numberOfInbox = messageUnitOfWork.getAllMessage(receiverID).Count();
            model.numberOfSent = messageUnitOfWork.getAllSentMessage(receiverID).Count();
            model.numberOfDeleted = model.messageList.Count();
            return View("List", model);
        }

        //public ActionResult SentMessageList(string messageCategory)
        //{
        //    MessageViewModel model = new MessageViewModel();
        //    string receiverID = messageUnitOfWork.AspNetUserRepository.Get(s => s.UserName == User.Identity.Name).FirstOrDefault().Id; 
        //    if ("allMessage".Equals(messageCategory))
        //    {
        //        model.messageList = messageUnitOfWork.getAllMessage(receiverID);
        //    }
        //    if ("sentMessage".Equals(messageCategory))
        //    {
        //        model.messageList = messageUnitOfWork.getAllSentMessage(receiverID);
        //        return PartialView("_MessageSentPartial", model);
        //    }
        //    if ("deletedMessage".Equals(messageCategory))
        //    {
        //        model.messageList = messageUnitOfWork.getAllDeleteMessage(receiverID);
        //    }
            
          
          
        //    return PartialView("_MessagePartial", model);
        //}


        public ActionResult DeleteMessage(string [] mark, MessageViewModel model)
        {
            string userId = messageUnitOfWork.AspNetUserRepository.Get(s => s.UserName == User.Identity.Name).FirstOrDefault().Id; 
            //ArrayList list = new ArrayList();
            //List<int> list = listMessageDelete.Split(',').Select(int.Parse).ToList();

            //List<string> listString = listMessageDelete.Split(new char[] { ',' });
            //list.Add();
            List<int> list = new List<int>();
            foreach (string item in mark)
            {
                int index = Int32.Parse(item);
                list.Add(index);
            }

            messageUnitOfWork.deleteMessage(userId, list, model.typeOfMessage);
            if (("sentMessage").Equals(model.typeOfMessage)){
                  return RedirectToAction("SentMessageList");
            }else{
                return RedirectToAction("List");
            }
            
        }

        public ActionResult Detail(int? id, string typeOfMessage)
        {
            string receiverID = messageUnitOfWork.AspNetUserRepository.Get(s => s.UserName == User.Identity.Name).FirstOrDefault().Id; 
            int messageId = id.GetValueOrDefault();
            if (messageId == 0 || String.IsNullOrEmpty(typeOfMessage))
            {

                return RedirectToAction("List", "Message");
            }
            else if (!messageUnitOfWork.IsMessageExist(messageId))
            {
                return RedirectToAction("List", "Message");
            }
            else
            {
                JMessageDetailViewModel jMessageDetailViewModel= new JMessageDetailViewModel();
                jMessageDetailViewModel.message = messageUnitOfWork.GetMessageDetail(receiverID, messageId, typeOfMessage);
                jMessageDetailViewModel.numberOfInbox = messageUnitOfWork.getAllMessage(receiverID).Count();
                jMessageDetailViewModel.numberOfSent = messageUnitOfWork.getAllSentMessage(receiverID).Count();
                jMessageDetailViewModel.numberOfDeleted = messageUnitOfWork.getAllDeleteMessage(receiverID).Count(); ;
                return View(jMessageDetailViewModel);
            }
         
        }


        public JsonResult CheckReceiverList(string userList)
        {
            ArrayList list = new ArrayList();
            list.AddRange(userList.Split(new char[] { ',' }));
            for (int i = 0; i < list.Count; i++)
            {
                for (int j = i + 1; j < list.Count; j++)
                {
                    if (list[i].ToString() == list[j].ToString())
                    {
                        list.Remove(list[j]);
                    }
                }
            }

            bool result = messageUnitOfWork.CheckReceiverExist(list);

            return Json(result, JsonRequestBehavior.AllowGet);

        }


        public JsonResult AutoCompleteUser(string username)
        {

            var result = messageUnitOfWork.AutoCompleteUser(username);

            return Json(result, JsonRequestBehavior.AllowGet);      
        }

        public ActionResult SendMessage(string receiver, string messageContent)
        {
            ArrayList list = new ArrayList();
            list.AddRange(receiver.Split(new char[] { ',' }));
            for (int i = 0; i < list.Count; i++)
            {
                for (int j = i + 1; j < list.Count; j++)
                {
                    if (list[i].ToString() == list[j].ToString())
                    {
                        list.Remove(list[j]);
                    }
                }
            }
                 

            bool result = messageUnitOfWork.SendMessage(User.Identity.Name, list, messageContent);
            if (result)
            {
                TempData["successmessage"] = "Tin nhắn của bạn đã được gửi đi.";
            }
            else
            {
                TempData["errormessage"] = "Gửi tin nhắn thất bại.";
            }
            
            return RedirectToAction("List");
        }

        public ActionResult SendMessageInterview(string sender, string receiver, string messageContent)
        {
            ArrayList list = new ArrayList();
            list.AddRange(receiver.Split(new char[] { ',' }));
            for (int i = 0; i < list.Count; i++)
            {
                for (int j = i + 1; j < list.Count; j++)
                {
                    if (list[i].ToString() == list[j].ToString())
                    {
                        list.Remove(list[j]);
                    }
                }
            }


            messageUnitOfWork.SendMessage(sender, list, messageContent);
            return RedirectToAction("List");
        }

	}
}
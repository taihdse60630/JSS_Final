using JobSearchingSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using JobSearchingSystem.DAL;
using System.Collections;

namespace JobSearchingSystem.DAL
{
    public class MessageUnitOfWork:UnitOfWork
    {

        public  IEnumerable<JMesssage> getAllMessage(string p)
        {
            var messageList = (from a in this.MessageRepository.Get()
                    join b in this.MessageReceiverRepository.Get() on a.MessageID equals b.MessageID
                    join c in this.AspNetUserRepository.Get() on a.SenderID equals c.Id
                    where b.ReceiverID == p && b.IsDeleted == false
                    select new JMesssage()
                    {
                        MessageID = a.MessageID,
                        SenderID = a.SenderID,
                        SenderName = c.UserName,
                        Content = a.Message_content,
                        SendTime = a.SendTime
                    }).AsEnumerable();

            messageList = messageList.Reverse();
            return messageList;
        }

        public List<string> AutoCompleteUser(string term)
        {
            var list = (from a in this.AspNetUserRepository.Get()
                    
                    select new { a.UserName }).ToArray();
            List<string> listUser = new List<string>();
            foreach (var item in list)
            {
                listUser.Add(item.UserName);
            }
            return listUser;
        }

        public void SendMessage(string sender, ArrayList receiver, string messageContent)
        {

            List<string> receiverList = new List<string>();
            foreach (var item in receiver)
            {
                string userId = getAspNetUserIdByName(item.ToString());
                if (userId != null)
                {
                    receiverList.Add(userId);
                }
            }

            if (receiverList.ToArray().Length > 0)
            {
                Message messageSender = new Message();
                messageSender.SenderID = getAspNetUserIdByName(sender);
                messageSender.Message_content = messageContent;
                messageSender.IsCanceled = false;
                messageSender.IsDeletedBySender = false;
                messageSender.SendTime = DateTime.Now;
                MessageRepository.Insert(messageSender);
                Save();

                int id = MessageRepository.Get().LastOrDefault().MessageID;

                MessageReceiver messageReceiver = null;
                foreach (var item in receiverList)
                {
                    messageReceiver = new MessageReceiver();
                    messageReceiver.ReceiverID = item.ToString();
                    messageReceiver.IsDeleted = false;
                    messageReceiver.MessageID = id;

                    MessageReceiverRepository.Insert(messageReceiver);
                    Save();
                }
               
           
            }
            else
            {
               
            }

        
            
        }

        public string getAspNetUserIdByName(string name)
        {
            AspNetUser user = AspNetUserRepository.Get(filter: m => m.UserName == name).FirstOrDefault();
            if (user != null)
            {
                return user.Id;
            }
            else
            {
                return null;
            }
           
    
        }

        public bool IsMessageExist(int messageId)
        {
            Message message=  MessageRepository.GetByID(messageId);
            if (message != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public JMesssage GetMessageDetail(string receiverId, int messageId)
        {
            JMesssage message = (from a in this.MessageRepository.Get()
                                 join b in this.MessageReceiverRepository.Get() on a.MessageID equals b.MessageID
                                 join c in this.AspNetUserRepository.Get() on a.SenderID equals c.Id
                                 where a.MessageID == messageId 
                                 select new JMesssage()
                                 {
                                     MessageID = a.MessageID,
                                     Content = a.Message_content,
                                     SenderName = c.UserName,
                                     SenderID = a.SenderID,
                                     SendTime = a.SendTime
                                 }).FirstOrDefault();
            return message;
        }

        public void deleteMessage(string userId, List<int> list, string messageRole)
        {
            List<MessageReceiver> messageList = new List<MessageReceiver>();
            List<Message> messageSendList = new List<Message>();
            if ("allMessage".Equals(messageRole))
            {
                foreach (var item in list)
                {

                    var message = MessageReceiverRepository.Get(filter: m => m.ReceiverID == userId && m.MessageID == item).FirstOrDefault();
                    message.IsDeleted = true;
                    messageList.Add(message);
                }

                foreach (var item in messageList)
                {
                    MessageReceiverRepository.Update(item);
                    Save();
                }
            }

            if ("sentMessage".Equals(messageRole))
            {
                foreach (var item in list)
                {

                    var message = MessageRepository.Get(filter: m => m.SenderID == userId && m.MessageID == item).FirstOrDefault();
                    message.IsDeletedBySender = true;
                    messageSendList.Add(message);
                }

                foreach (var item in messageSendList)
                {
                    MessageRepository.Update(item);
                    Save();
                }
            }
        
            
        }

        public IEnumerable<JMesssage> getAllSentMessage(string receiverID)
        {
            List<string> receiverList = null;
            var messageList = (from a in this.MessageRepository.Get()
                               join b in this.MessageReceiverRepository.Get() on a.MessageID equals b.MessageID
                               join c in this.AspNetUserRepository.Get() on b.ReceiverID equals c.Id
                               where a.SenderID == receiverID && a.IsDeletedBySender == false
                               select new JMesssage()
                               {
                                   MessageID = a.MessageID,
                                   SenderID = a.SenderID,
                                   ReceiverName = c.UserName,
                                   Content = a.Message_content,
                                   SendTime = a.SendTime
                               }).AsEnumerable();

            var messageSentList =(from a in this.MessageRepository.Get()                               
                               where a.SenderID == receiverID && a.IsDeletedBySender == false
                               select new JMesssage()
                               {
                                   MessageID = a.MessageID,
                                   SenderID = a.SenderID,                                  
                                   Content = a.Message_content,
                                   SendTime = a.SendTime
                               }).ToArray();

            for (int i = 0; i < messageSentList.Length; i++)
            {
                receiverList = new List<string>();
                foreach (var item2 in messageList)
                {
                    if (item2.SenderID.Equals(messageSentList[i].SenderID) && item2.MessageID == messageSentList[i].MessageID)
                    {
                        receiverList.Add(item2.ReceiverName);
                    }
                }
                messageSentList[i].ReceiverList = receiverList;
            }

           IEnumerable<JMesssage> messageReverse = messageSentList.Reverse();
           return messageReverse;
        }

        public IEnumerable<JMesssage> getAllDeleteMessage(string userId)
        {
            var messageList = (from a in this.MessageRepository.Get()
                               join b in this.MessageReceiverRepository.Get() on a.MessageID equals b.MessageID
                               join c in this.AspNetUserRepository.Get() on a.SenderID equals c.Id
                               where b.ReceiverID == userId && b.IsDeleted == true
                               select new JMesssage()
                               {
                                   MessageID = a.MessageID,
                                   SenderID = a.SenderID,
                                   SenderName = c.UserName,
                                   Content = a.Message_content,
                                   SendTime = a.SendTime
                               }).AsEnumerable();

            var messageSenderList = (from a in this.MessageRepository.Get()
                                     join b in this.MessageReceiverRepository.Get() on a.MessageID equals b.MessageID
                                     join c in this.AspNetUserRepository.Get() on a.SenderID equals c.Id
                                     where a.SenderID == userId && a.IsDeletedBySender == true
                                     select new JMesssage()
                                     {
                                         MessageID = a.MessageID,
                                         SenderID = a.SenderID,
                                         SenderName = c.UserName,
                                         Content = a.Message_content,
                                         SendTime = a.SendTime
                                     }).AsEnumerable();

            List<JMesssage> messageDeleteList = new List<JMesssage>();

            foreach(var item in messageList){
                messageDeleteList.Add(item);
            }

            foreach (var item in messageSenderList)
            {
                messageDeleteList.Add(item);
            }

            messageDeleteList.Reverse(0,messageDeleteList.Count);

            return messageDeleteList;
        }
    }
}
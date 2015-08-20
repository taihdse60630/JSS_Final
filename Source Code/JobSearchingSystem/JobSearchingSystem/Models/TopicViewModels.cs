using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using JobSearchingSystem.DAL;

namespace JobSearchingSystem.Models
{

    public class TopicItem
    {
        public int TopicID { get; set; }
        public string Title { get; set; }
        public string ImageUrl { get; set; }
        public string WriterID { get; set; }
        public string Topic_content { get; set; }
        public DateTime CreatedDate { get; set; }
        public Nullable<DateTime> UpdatedDate { get; set; }
        public string UpdateStaffID { get; set; }
        public Nullable<bool> IsApprove { get; set; }
        public bool IsDelete { get; set; }

        public TopicItem(int topicID, string title, string imageUrl, string writerID, string topic_content, DateTime createdDate,
            Nullable<DateTime> updatedDate, string updateStaffID, Nullable<bool> isApprove, bool isDelete)
        {
            this.TopicID = topicID;
            this.Title = title;
            this.ImageUrl = imageUrl;
            this.WriterID = writerID;
            this.Topic_content = topic_content;
            this.CreatedDate = createdDate;
            this.UpdatedDate = updatedDate;
            this.UpdateStaffID = updateStaffID;
            this.IsApprove = isApprove;
            this.IsDelete = isDelete;
        }

        public TopicItem(Topic topic)
        {
            this.TopicID = topic.TopicID;
            this.Title = topic.Title;
            this.ImageUrl = topic.ImageUrl;
            this.WriterID = topic.WriterID;
            this.Topic_content = topic.Topic_content;
            this.CreatedDate = topic.CreatedDate;
            this.UpdatedDate = topic.UpdatedDate;
            this.UpdateStaffID = topic.UpdatedStaffID;
            this.IsApprove = topic.IsApproved;
            this.IsDelete = topic.IsDeleted;
        }
    }

    //Model using for both Create and Update new Topic
    public class TopCreateViewModel
    {
        public int TopicID { get; set; }
        public string Title { get; set; }
        public string WriterID { get; set; }
        public string ImageUrl { get; set; }
        public string Topic_content { get; set; }
        public DateTime CreatedDate { get; set; }
        public Nullable<DateTime> UpdatedDate { get; set; }
        public string UpdateStaffID { get; set; }
        public Nullable<bool> IsApprove { get; set; }
        public bool IsDelete { get; set; }
        public bool IsUpdated { get; set; }

        //CONTROLLER START
        public TopCreateViewModel()
        {
            TopicID = 0;
            Title = "";
            WriterID = "";
            ImageUrl = "";
            Topic_content = "";
            CreatedDate = DateTime.Now;
            UpdatedDate = null;
            UpdateStaffID = null;
            IsApprove = null;
            IsDelete = false;
            IsUpdated = false;
        }

        public TopCreateViewModel(int id, string title, string writerId, string url, string content, DateTime createdDate,
            Nullable<DateTime> updatedDate, string updaterID, Nullable<bool> approve, bool delete, bool isUpdated)
        {
            this.TopicID = id;
            this.Title = title;
            this.WriterID = writerId;
            this.ImageUrl = url;
            this.Topic_content = content;
            this.CreatedDate = createdDate;
            this.UpdatedDate = updatedDate;
            this.UpdateStaffID = updaterID;
            this.IsApprove = approve;
            this.IsDelete = delete;
            this.IsUpdated = isUpdated;
        }
        
        public TopCreateViewModel(Topic item)
        {
            this.TopicID = item.TopicID;
            this.Title = item.Title;
            this.WriterID = item.WriterID;
            this.ImageUrl = item.ImageUrl;
            this.Topic_content = item.Topic_content;
            this.CreatedDate = item.CreatedDate;
            this.UpdatedDate = item.UpdatedDate;
            this.UpdateStaffID = item.UpdatedStaffID;
            this.IsApprove = item.IsApproved;
            this.IsDelete = item.IsDeleted;
            this.IsUpdated = item.IsUpdated;
        }
        //CONTROLLER END

        
    }

    public class TopListViewModel
    {
        public IEnumerable<TopicItem> ListTopic { get; set; }

        public TopListViewModel()
        {
            ListTopic = new List<TopicItem>();
        }

        public TopListViewModel(IEnumerable<Topic> list)
        {
            List<TopicItem> temp = new List<TopicItem>();
            foreach (Topic item in list)
            {
                temp.Add(new TopicItem(item));
            }
            ListTopic = temp;
        }
    }

    public class TopicFunctionModel
    {
        private TopicUnitOfWork topicUnitOfWork = new TopicUnitOfWork();

        //FUNCTION START
        
        //Get Topic by ID (using for update view)
        public TopCreateViewModel GetTopicByID(int id)
        {
            return (new TopCreateViewModel(topicUnitOfWork.GetByID(id)));
        }

        //Get Topic by ID (using for detail view)
        public TopicItem GetTopicDetailByID(int id)
        {
            return new TopicItem(topicUnitOfWork.GetByID(id));
        }

        //Return list of Topic
        public TopListViewModel GetListTopic()
        {
            return (new TopListViewModel(topicUnitOfWork.Get()));
        }

        //Get list of Topic by UserID
        public TopListViewModel GetListByUserID(string id)
        {
            return new TopListViewModel(topicUnitOfWork.GetByUserID(id));
        }

        //Create new topic
        public bool CreateTopic(TopCreateViewModel model)
        {

            Topic newTopic = new Topic();
            newTopic.WriterID = model.WriterID;
            newTopic.Title = model.Title;
            newTopic.Topic_content = model.Topic_content;
            newTopic.CreatedDate = DateTime.Now;
            newTopic.UpdatedDate = null;
            newTopic.UpdatedStaffID = null;
            newTopic.IsApproved = null; //Must be converted into null Variable;
            newTopic.IsDeleted = false;
            newTopic.ImageUrl = model.ImageUrl;
            newTopic.IsUpdated = false;

            return topicUnitOfWork.Create(newTopic);
        }

        //Update topic
        public bool UpdateTopic(TopCreateViewModel model)
        {
            Topic updateTopic = topicUnitOfWork.GetByID(model.TopicID);
            updateTopic.WriterID = model.WriterID;
            updateTopic.Title = model.Title;
            updateTopic.Topic_content = model.Topic_content;
            updateTopic.CreatedDate = model.CreatedDate;
            updateTopic.UpdatedDate = model.UpdatedDate;
            updateTopic.UpdatedStaffID = model.UpdateStaffID;
            updateTopic.IsApproved = model.IsApprove; //Must be converted into null Variable;
            updateTopic.IsDeleted = model.IsDelete;
            updateTopic.ImageUrl = model.ImageUrl;
            updateTopic.IsUpdated = true;

            return topicUnitOfWork.Update(updateTopic);
        }

        //Delete topic
        public bool DeleteTopic(int topicID)
        {
            return topicUnitOfWork.Delete(topicID);
        }

        //Get pending list
        public TopListViewModel GetPendingList()
        {
            return topicUnitOfWork.GetPendingList();
        }

        //Approve Topic
        public bool ApproveTopic(int topicID)
        {
            return topicUnitOfWork.Approve(topicID);
        }

        //Disapprove Topic
        public bool DisapproveTopic(int topicID)
        {
            return topicUnitOfWork.Disapprove(topicID);
        }

        //FUNCTION END
    }
}
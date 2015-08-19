using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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

    public class TopCreateViewModel
    {
        public int TopicID { get; set; }
        public string WriterID { get; set; }
        public string Topic_content { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string UpdateStaffID { get; set; }
        public bool IsApprove { get; set; }
        public bool IsDelete { get; set; }
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
}
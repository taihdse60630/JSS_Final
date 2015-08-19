using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using JobSearchingSystem.Models;

namespace JobSearchingSystem.DAL
{
    public class TopicUnitOfWork : UnitOfWork
    {
        //Create new topic
        public bool Create(Topic newTopic)
        {
            if (newTopic != null)
            {
                this.TopicRepository.Insert(newTopic);
                this.Save();
                return true;
            }
            return false;
        }

        //Get all topic in List
        public IEnumerable<Topic> Get()
        {
            return this.TopicRepository.Get();
        }

        //Get topic by id
        public Topic GetByID(int id)
        {
            return this.TopicRepository.GetByID(id);
        }

        //Get all topic in list by user ID
        public IEnumerable<Topic> GetByUserID(string id)
        {
            return this.TopicRepository.Get(filter: topic => topic.AspNetUser.Id == id && topic.IsDeleted == false);
        }

        //Delete topic
        public bool Delete(int topicID)
        {
            try
            {
                Topic topic = this.TopicRepository.GetByID(topicID);
                topic.IsDeleted = true;
                TopicRepository.Update(topic);
                this.Save();
                return true;
            }
            catch
            {
                return false;
            }
        }

        //Get list of topics waiting for approved
        public TopListViewModel GetPendingList()
        {
            return new TopListViewModel(this.TopicRepository.Get(filter: topic => topic.IsDeleted == false && topic.IsApproved == null));
        }

        //Approve topic
        public bool Approve(int topicID)
        {
            try
            {
                Topic topic = this.TopicRepository.GetByID(topicID);
                topic.IsApproved = true;
                TopicRepository.Update(topic);
                this.Save();
                return true;
            }
            catch
            {
                return false;
            }
        }

        //Disapprove topic
        public bool Disapprove(int topicID)
        {
            try
            {
                Topic topic = this.TopicRepository.GetByID(topicID);
                topic.IsApproved = false;
                TopicRepository.Update(topic);
                this.Save();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
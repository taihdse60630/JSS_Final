using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JobSearchingSystem.Models
{
    public class JMesssage
    {
        public int MessageID { get; set; }
        public string SenderID { get; set; }
        public string Content { get; set; }
        public DateTime? SendTime { get; set; }
        public string SenderName { get; set; }
        public List<string> ReceiverList { get; set; }
        public string ReceiverName { get; set; }
    }
    public class MessageViewModel
    {
        public IEnumerable<JMesssage> messageList { get; set; }
        public string typeOfMessage { get; set; }
    }

    public class JMessageDetailViewModel
    {
        public JMesssage message { get; set; }
    }
}
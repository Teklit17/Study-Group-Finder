using System.Collections.Generic;

namespace SG_Finder.Models
{
    public class MessageBellViewModel
    {
        public int UnreadMessageCount { get; set; } // Total number of unread messages
        public List<Message> UnreadMessages { get; set; } // List of unread messages
    }
}
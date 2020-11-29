using System;

namespace Chatex.Models
{
    public class ChatLisData
    {
        public int AccountId { get; set; }
        public string ContactName { get; set; }
        public Uri ContactPhoto { get; set; }
        public string Message { get; set; }
        public string LastMessageTime { get; set; }
        public bool ChatIsSelected { get; set; }
        public bool ChatIsPinned { get; set; }

    }
}
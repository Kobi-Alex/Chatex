using System;

namespace Chatex.Models
{
    public class ChatLisData
    {
        public string ContactName { get; set; }
        public Uri ContactPhoto { get; set; }
        public string Message { get; set; }
        public string LastMessageTime { get; set; }
        public bool ChatIsSelected { get; set; }

    }
}
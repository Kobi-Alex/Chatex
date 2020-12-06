using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace ServerAssistant
{
    [Serializable]
    public enum TypeRequest
    {
        authentication = 1,
        text = 2,
        image = 3,
        sound = 4,
        logOut = 5,
        loadData = 6
            
    }

    [Serializable]
    public class Request
    {
        // who will be receiver message
        public string Receiver { get; set; }
        public TypeRequest TypeRequest { get; set; }
        public object Data { get; set; }

        public Request()
        {

        }

        //For Login & Password
        public Request(string receiver, TypeRequest typeRequest, object data)
        {
            this.Receiver = receiver;
            this.TypeRequest = typeRequest;
            this.Data = data;
        }

        public Request(TypeRequest typeRequest, object data)
        {
            this.TypeRequest = typeRequest;
            this.Data = data;
        }

        public Request(TypeRequest typeRequest)
        {
            this.TypeRequest = typeRequest;
        }
    }
}

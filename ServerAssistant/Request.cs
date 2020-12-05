using System;
using System.Collections.Generic;
using System.Text;

namespace ServerAssistant
{
    [Serializable]
    public enum TypeRequest
    {
        authentication = 1,
    }

    [Serializable]
    public class Request
    {
        public int Receiver { get; set; }
        public TypeRequest TypeRequest { get; set; }
        public object Data { get; set; }

        public Request()
        {

        }

        //For Login & Password
        public Request(int receiver, TypeRequest typeRequest, object data)
        {
            this.Receiver = receiver;
            this.TypeRequest = typeRequest;
            this.Data = data;
        }

    }


}

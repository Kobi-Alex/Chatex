using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;

namespace RequestsDLL
{
    [Serializable]
    public enum TypeRequest
    {
        authentication = 1,
    }

    [Serializable]
    public class ServerRequest
    {
        public int Receiver { get; set; }
        public TypeRequest TypeRequest { get; set; }
        public object Data { get; set; }

        public ServerRequest()
        {

        }

        //For Login & Password
        public ServerRequest(int receiver, TypeRequest typeRequest, string data)
        {
            this.Receiver = receiver;
            this.TypeRequest = typeRequest;
            this.Data = data;
        }


        public static ServerRequest ByteArrayToObect(byte[] arrBytes)
        {
            using (var memoryStream = new MemoryStream())
            {
                var binForm = new BinaryFormatter();
                memoryStream.Write(arrBytes, 0, arrBytes.Length);
                memoryStream.Seek(0, SeekOrigin.Begin);
                var obj = binForm.Deserialize(memoryStream);

                return obj as ServerRequest;
            }
        }

        public static byte[] ObjectToByteArray(ServerRequest obj)
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (var memoryStream = new MemoryStream())
            {
                bf.Serialize(memoryStream, obj);
                return memoryStream.ToArray();
            }
        }
    }
}

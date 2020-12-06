using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;



namespace ServerAssistant
{
    public class Assistant
    {
        public TcpClient Client { get; private set; }

        private int id;
        public int Id { get => id; set => id = value; }


        public Assistant()
        {

        }


        // Server connection
        public void ConnectToServer(int portUser)
        {
            IPEndPoint clientEP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), portUser);
            TcpClient client = new TcpClient(clientEP);

            IPEndPoint serverEP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1024);
            client.Connect(serverEP);

            this.Client = client;
        }

        public void SignRequestToServer(Request data)
        {
            BinaryWriter writer = new BinaryWriter(Client.GetStream());

            byte[] objectArr = ObjectToByteArray(data);
            writer.Write(objectArr.Length);
            writer.Write(objectArr);
            writer.Flush();
        }

        public void RecivedDataFromServer()
        {
            
        }




        public static Request ByteArrayToObect(byte[] arrBytes)
        {
            using (var memoryStream = new MemoryStream())
            {
                var binForm = new BinaryFormatter();
                memoryStream.Write(arrBytes, 0, arrBytes.Length);
                memoryStream.Seek(0, SeekOrigin.Begin);
                var obj = binForm.Deserialize(memoryStream);

                return obj as Request;
            }
        }
        public static byte[] ObjectToByteArray(Request obj)
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

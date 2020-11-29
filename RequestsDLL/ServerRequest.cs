using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
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

        public string Login { get; set;}
        public string Password { get; set;}
        public object Data { get; set; }
        public TcpClient Client { get; set; }
        public TypeRequest TypeRequest { get; set; }



        private BinaryWriter writer;
        private BinaryReader reader;

        public ServerRequest()
        {

        }

        //For Login & Password
        public ServerRequest(string login, string password, TcpClient client, TypeRequest typeRequest)
        {
            this.Login = login;
            this.Password = password;
            this.Client = client;
            this.TypeRequest = typeRequest;

        }


        public bool ClientAuthentication(ServerRequest serverRequest)
        {
            ReceiveReplyFromServer(Client);
            SendRequestToServer(serverRequest);

            return (bool)Data;
        }


        private void SendRequestToServer(ServerRequest serverRequest)
        {
            Task.Run(() =>
            {
                if (serverRequest != null)
                {
                    try
                    {
                        byte[] objectArr = ServerRequest.ObjectToByteArray(serverRequest);
                        writer.Write(objectArr.Length);
                        writer.Write(objectArr);
                        writer.Flush();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error send file to server  " + ex.Message);
                    }
                }
            });
        }

        private void ReceiveReplyFromServer(TcpClient client)
        {
            Task.Run(() =>
            {
                NetworkStream dataStream = client.GetStream();
                writer = new BinaryWriter(dataStream);
                reader = new BinaryReader(dataStream);

                byte[] buffer = new byte[1000];

                while (client.Connected)
                {
                    int length = reader.ReadInt32();
                    buffer = reader.ReadBytes(length);

                    ServerRequest serverRequest = ServerRequest.ByteArrayToObect(buffer);

                    switch (serverRequest.TypeRequest)
                    {
                        // reply authetication from server 
                        case TypeRequest.authentication:
                            {
                               this.Data = (bool)serverRequest.Data;
                            }
                            break;
                        default:
                            break;
                    }
                }
            });
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

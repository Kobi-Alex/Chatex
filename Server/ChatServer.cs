using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.IO.Pipes;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using ChatModelsDLL;
using ServerAssistant;
using System.Data.SqlClient;

namespace Server
{
    class ChatServer
    {
        private object locker = new object();
        private TcpListener server;
        private List<TcpClient> connectedClients;
        private string currentClientPort;
        ChatexDBEntities chatexDBEntities = new ChatexDBEntities();
        Dictionary<string, int> serverAuData = new Dictionary<string, int>();

        public ChatServer(IPAddress serverIp, int port)
        {
            IPEndPoint serverEP = new IPEndPoint(serverIp, port);
            this.server = new TcpListener(serverEP);
            connectedClients = new List<TcpClient>();
           // chatexDBEntities = new ChatexDBEntities();
        }

        public void Start()
        {
            server.Start();
            while (true)
            {
                TcpClient client = server.AcceptTcpClient();

                lock (this.locker)
                {
                    this.connectedClients.Add(client);
                }
                Task.Run(() => ClientMethodAsync(client));
            }
        }

        //private void AuthenticationMethodAsync(object state)
        //{
        //    TcpClient client = (TcpClient)state;

        //    NetworkStream dataStream = client.GetStream();
        //    BinaryReader reader = new BinaryReader(dataStream);

        //    while (true)
        //    {
        //        byte[] buffer = new byte[1000];
        //        try
        //        {
        //            while (client.Connected)
        //            {
        //                int length = reader.ReadInt32();
        //                buffer = reader.ReadBytes(length);

        //                currentClientPort = client.Client.RemoteEndPoint.ToString();
        //                Console.WriteLine(client.Client.RemoteEndPoint.ToString());
                        
        //                //отримали дані
        //                ServerRequest serverRequest = ServerRequest.ByteArrayToObect(buffer);

        //                //logic Authentication
        //                if (serverRequest.TypeRequest == TypeRequest.authentication)
        //                {
        //                    //var dataL = chatexDBEntities.Account.Find(serverRequest.Login);
        //                    //var dataP = chatexDBEntities.Account.Find(serverRequest.Password);

        //                    //if (dataL != null && dataP != null)
        //                    //{ 

        //                    //    ClientMethod(client);
        //                    //}
        //                }
        //                else 
        //                    break;

        //            }
        //        }
        //        catch (IOException)
        //        {
        //            lock (locker)
        //            {
        //                this.connectedClients.Remove(client);
        //            }
        //            break;
        //        }
        //        finally
        //        {
        //            reader.Close();
        //        }
        //    }
        //}

        private void ClientMethodAsync(TcpClient client)
        {
            NetworkStream dataStream = client.GetStream();
            BinaryReader reader = new BinaryReader(dataStream);
            Assistant assistant = new Assistant();

            while (true)
            {
                byte[] buffer = new byte[10000];
                try
                {
                    while (client.Connected)
                    {
                        int length = reader.ReadInt32();
                        buffer = reader.ReadBytes(length);

                        currentClientPort = client.Client.RemoteEndPoint.ToString();
                        Console.WriteLine(client.Client.RemoteEndPoint.ToString());

                        //отримали дані
                        Request serverRequest = Assistant.ByteArrayToObect(buffer);

                        //Logics 
                        switch (serverRequest.TypeRequest)
                        {
                            case TypeRequest.authentication:
                                {
                                    Tuple<string, string> data = (Tuple<string, string>)serverRequest.Data;

                                    string login = data.Item1;
                                    string password = data.Item2;

                                    Account user = chatexDBEntities.Account.Where(c => c.Login == login && c.Password == password).FirstOrDefault();

                                    if (user !=null)
                                    { 
                                        serverAuData.Add(client.Client.RemoteEndPoint.ToString(), user.Id);

                                        serverRequest.Data = true; 
                                    }
                                    else {serverRequest.Data = false;}

                                }
                                break;

                            case TypeRequest.text:
                                {
                                    string data = (string)serverRequest.Data;
                                    string key = client.Client.RemoteEndPoint.ToString();

                                    Conversation conversation = new Conversation
                                    {
                                        AccountID = serverAuData.Where(item => item.Key == key).Select(i => i.Value).FirstOrDefault(),
                                        LastOnline = DateTime.Now.Date,
                                        SentMsg = data,
                                        MsgSentOn = DateTime.Now
                                    };

                                    chatexDBEntities.Conversation.Add(conversation);
                                    chatexDBEntities.SaveChanges(); 
                                }
                                break;
                            case TypeRequest.loadData:
                                {
                                    string key = client.Client.RemoteEndPoint.ToString();
                                    int Id = serverAuData.Where(item => item.Key == key).Select(i => i.Value).FirstOrDefault();

                                    var Data = chatexDBEntities.ChatList.AsNoTracking().Where(a => a.AccountID == Id).
                                        Join(chatexDBEntities.Account, c => c.AccountID, (ac) => ac.Id, (c, ac) => ac).ToList();
                                        //Select(ac => new { ac.Id, ac.Login, ac.ContactName, ac.ContactPhoto }).ToList();




                                    serverRequest.Data = Data;
                                }
                                break;
                        }

                        buffer = Assistant.ObjectToByteArray(serverRequest);
                        Task.Run(() => ResenderAsync(buffer));
                    }
                }
                catch (IOException)
                {
                    lock (locker)
                    {
                        this.connectedClients.Remove(client);
                    }
                    break;
                }
                finally
                {
                    reader.Close();
                }
            }
        }


        public void ResenderAsync(object state)
        {
            byte[] buffer = (byte[])state;

            //Request serverRequest = Assistant.ByteArrayToObect(buffer);
           // string port = serverRequest.Receiver;

            lock (this.locker)
            {
                int length = this.connectedClients.Count;
                for (int i = 0; i < length; i++)
                {
                    if (this.connectedClients[i].Connected)
                    {
                       // if (this.connectedClients[i].Client.RemoteEndPoint.ToString() == port)
                        {
                            BinaryWriter writer = new BinaryWriter(connectedClients[i].GetStream());
                            writer.Write(buffer.Length);
                            writer.Write(buffer);
                            writer.Flush();
                        }
                    }
                    else
                    {
                        this.connectedClients.RemoveAt(i--);
                        length--;
                    }
                }
            }

        }
        public void Stop()
        {
            server.Stop();
        }
    }
}

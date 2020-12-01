using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.IO.Pipes;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using RequestsDLL;
using ChatModelsDLL;

namespace Server
{
    class ChatServer
    {
        private object locker = new object();
        private TcpListener server;
        private List<TcpClient> connectedClients;
        private string currentClientPort;
        ChatexDBEntities chatexDBEntities = new ChatexDBEntities();

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

        private void AuthenticationMethodAsync(object state)
        {
            TcpClient client = (TcpClient)state;

            NetworkStream dataStream = client.GetStream();
            BinaryReader reader = new BinaryReader(dataStream);

            while (true)
            {
                byte[] buffer = new byte[1000];
                try
                {
                    while (client.Connected)
                    {
                        int length = reader.ReadInt32();
                        buffer = reader.ReadBytes(length);

                        currentClientPort = client.Client.RemoteEndPoint.ToString();
                        Console.WriteLine(client.Client.RemoteEndPoint.ToString());
                        
                        //отримали дані
                        ServerRequest serverRequest = ServerRequest.ByteArrayToObect(buffer);

                        //logic Authentication
                        if (serverRequest.TypeRequest == TypeRequest.authentication)
                        {
                            //var dataL = chatexDBEntities.Account.Find(serverRequest.Login);
                            //var dataP = chatexDBEntities.Account.Find(serverRequest.Password);

                            //if (dataL != null && dataP != null)
                            //{ 

                            //    ClientMethod(client);
                            //}
                        }
                        else 
                            break;

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

        private void ClientMethodAsync(TcpClient client)
        {
            NetworkStream dataStream = client.GetStream();
            BinaryReader reader = new BinaryReader(dataStream);


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
                        ServerRequest serverRequest = ServerRequest.ByteArrayToObect(buffer);

                        //Logics 
                        switch (serverRequest.TypeRequest)
                        {
                            case TypeRequest.authentication:
                                {
                                    string pass = (string)serverRequest.Data;
                                    string login =chatexDBEntities.Account.Where(c => c.Password == pass).Select(s => s.Login).FirstOrDefault();


                                    if (login != null)
                                    {
                                       ///ServerRequest serverUnswer = new ServerRequest(portUser, TypeRequest.authentication, password);

                                        serverRequest.Data = login;
                                        buffer = ServerRequest.ObjectToByteArray(serverRequest);


                                        //Task.Run(() => ResenderUnswerAuthenticationAsync(serverUnswer, client));
                                    }
                                    //else
                                    //{
                                    //    serverRequest.Data = login;
                                    //    byte[] serverUnswer = ServerRequest.ObjectToByteArray(serverRequest);

                                    //    // Task.Run(() => ResenderUnswerAuthenticationAsync(serverUnswer, client));
                                    //    break;
                                    //}

                                }
                                break;
                                //case TypeRequest.authentication:
                                //    логіка отримання повідомлення
                                //    break;
                                //case TypeRequest.authentication:
                                //    логіка отримання повідомлення
                                //    break;
                        }

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


        public void ResenderUnswerAuthenticationAsync(object state, TcpClient client)
        {
            byte[] buffer = (byte[])state;

            lock (this.locker)
            {
                if (client.Connected)
                {
                    BinaryWriter writer = new BinaryWriter(client.GetStream());
                    writer.Write(buffer.Length);
                    writer.Write(buffer);
                    writer.Flush();
                }             
            }
        }



        public void ResenderAsync(object state)
        {
            byte[] buffer = (byte[])state;

            ServerRequest serverRequest = ServerRequest.ByteArrayToObect(buffer);

           int port = serverRequest.Receiver;

            string ReceiveIpAddress = $"127.0.0.1:{port}";

            lock (this.locker)
            {
                int length = this.connectedClients.Count;
                for (int i = 0; i < length; i++)
                {
                    if (this.connectedClients[i].Connected)
                    {
                        if (this.connectedClients[i].Client.RemoteEndPoint.ToString() == ReceiveIpAddress)
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

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

namespace Server
{
    class ChatServer
    {
        private object locker = new object();
        private TcpListener server;
        private List<TcpClient> connectedClients;
        private string currentClientPort;

        public ChatServer(IPAddress serverIp, int port)
        {
            IPEndPoint serverEP = new IPEndPoint(serverIp, port);
            this.server = new TcpListener(serverEP);
            connectedClients = new List<TcpClient>();
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
                Task.Run(() => AuthenticationMethodAsync(client));
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



                        //десеріалізували
                        TypeRequest type = TypeRequest.authentication;

                        switch (type)
                        {
                            //logic Authentication
                            case TypeRequest.authentication:
                                { 

                                
                                    ClientMethod(client);
                                
                                } break;
                            default:
                                break;
                        }
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

        private void ClientMethod(TcpClient client)
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

                        //десеріалізували
                        //TypeRequest type = TypeRequest.authentication;

                        //switch (type)
                        //{
                        //    //case TypeRequest.authentication:
                        //    //    //логіка аутентифікації



                        //    //    break;
                        //    //case TypeRequest.authentication:
                        //    //    //логіка отримання повідомлення
                        //    //    break;
                        //    //case TypeRequest.authentication:
                        //    //    //логіка отримання повідомлення
                        //    //    break;
                        //}

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

           // ServerMEssage serverMEssage = ServerMEssage.ByteArrayToObject(buffer);
           //int UserID = serverMEssage.Receiver;
           int UserID = 7;

            string ReceiveIpAddress = $"127.0.0.1:{(UserID + 1024)}";

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

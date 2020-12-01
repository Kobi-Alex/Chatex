using System;
using System.Net;
using System.Net.Sockets;

namespace ConnectDLL
{
    public class Connection
    {
        public Connection(int portUser, int portServer)
        {

            IPEndPoint clientEP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), portUser);
            TcpClient client = new TcpClient(clientEP);

            IPEndPoint serverEP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), portServer);
            client.Connect(serverEP);
        }
    }
}

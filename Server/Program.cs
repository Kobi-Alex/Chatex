using System;
using System.Net;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine("==================== Server srarted ==================");
            ChatServer server = new ChatServer(IPAddress.Parse("127.0.0.1"), 1024);
            server.Start();

            Console.WriteLine("Enter any key to stop...");
            Console.ReadKey();
            server.Stop();
        }
    }
}

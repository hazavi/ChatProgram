using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ChatClient
{
    internal class ChatClient
    {
        public string name;
        public string Name { get { return name; } set { name = value; } }
        public ChatClient()
        {
            Console.Write("Enter Your Name: ");
            Name = Console.ReadLine();

            IPAddress serverIp = GetServerIpAddress();
            IPEndPoint endPoint = new IPEndPoint(serverIp, 11000);
            while (true) StartClient(GetMessage(), endPoint);
        }

        private void StartClient(string msg, IPEndPoint endPoint)
        {
            Socket sender = new(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            Console.WriteLine($"Connecting to : {endPoint}");
            sender.Connect(endPoint);

            byte[] byteArray = System.Text.Encoding.ASCII.GetBytes(msg);
            sender.Send(byteArray);

            byte[] msgFromServer = new byte[1024];
            sender.Receive(msgFromServer);
            string msgRecieved = System.Text.Encoding.ASCII.GetString(msgFromServer);
            Console.WriteLine($"Message from server: {msgRecieved}");

            sender.Shutdown(SocketShutdown.Both);
            sender.Close();
        }

        private string GetMessage()
        {

            Console.Write($"{Name}: ");
            return Console.ReadLine() + "<EOM>";
        }

        private IPAddress GetServerIpAddress()
        {
            Console.Write("Connect to Server IP: ");
            IPAddress? ip = IPAddress.TryParse(Console.ReadLine(), out ip) ? ip : IPAddress.Parse("192.168.2.3");
            return ip;
        }


    }
}

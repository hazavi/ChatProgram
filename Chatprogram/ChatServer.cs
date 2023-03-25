using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ChatServer
{
    internal class ChatServer
    {


        public ChatServer()
        {
            //Endpoint consists of an IP address AND a port.
            IPEndPoint endpoint = GetServerEndpoint();
            //IPEndPoint endpoint = new IPEndPoint(IPAddress.Parse("192.168.2.3"), 11000);
            //Start server with endpoint previously selected.
            Socket listener = new(endpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            listener.Bind(endpoint);
            listener.Listen(10);
            Console.WriteLine($"Server Listening on: {listener.LocalEndPoint}");

            while (true) StartServer(listener);

        }



        private void StartServer(Socket listener)
        {
            Socket handler = listener.Accept();
            Console.WriteLine($"Accepting connection from {handler.RemoteEndPoint}");

            string? msg = null;
            byte[] buffer = new byte[1024];

            while (msg == null || !msg.Contains("<EOM>"))
            {
                int received = handler.Receive(buffer);
                msg += Encoding.ASCII.GetString(buffer, 0, received);
            }
            Console.WriteLine($"Message: {msg}");
            string msgRecieved = "Message Recieved";
            byte[] bytes = Encoding.ASCII.GetBytes(msgRecieved);
            handler.Send(bytes);
            handler.Shutdown(SocketShutdown.Both);
            handler.Close();
        }

        private IPEndPoint GetServerEndpoint()
        {
            //Gets the hostname of the machine
            string strHostName = Dns.GetHostName();
            //Uses hostnape to get host entry
            IPHostEntry host = Dns.GetHostEntry(strHostName);
            //Host entry contains all IP addressses
            //We create a list of IPv4 addresses
            List<IPAddress> addrList = new();
            int counter = 0;
            //Loops through host addresslist and adds to our list
            //if not IPv6
            foreach (var item in host.AddressList)
            {
                if (item.AddressFamily == AddressFamily.InterNetworkV6) continue;
                Console.WriteLine($"{counter++} {item.ToString()}");
                addrList.Add(item);
            }
            //Selects the IP from the list. If input is number and is within the range of the list,
            //If list contains 1 endpoint use that instead of asking.
            if (addrList.Count == 1) return new IPEndPoint(addrList[0], 11000);

            int temp;
            do Console.Write("Select server IP: ");
            while (!int.TryParse(Console.ReadLine(), out temp) || temp > addrList.Count || temp < 0);

            return new IPEndPoint(addrList[temp], 11000);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
namespace Lab1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Server";
            var hostName = Dns.GetHostName();
            var ipHostInfo = Dns.GetHostEntry(hostName);
            var ipAddress = ipHostInfo.AddressList.LastOrDefault(p => p.AddressFamily == AddressFamily.InterNetwork);
            var localEndPoint = new IPEndPoint(ipAddress, 10001);

            Server.IServer server = new Server.Server(new Server.SocketConnection(localEndPoint, ProtocolType.Tcp, SocketType.Stream, AddressFamily.InterNetwork));
            server.Start();
        }
    }
}

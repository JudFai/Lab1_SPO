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
        private static string EndCommandRow = Environment.NewLine;
        static void Main(string[] args)
        {
            Console.Write("Select type of application: server(s) or client(c): ");
            var key = Console.ReadKey();
            Console.WriteLine();
            if (key.Key == ConsoleKey.S)
            {
                var hostName = Dns.GetHostName();
                var ipHostInfo = Dns.GetHostEntry(hostName);
                var ipAddress = ipHostInfo.AddressList.LastOrDefault(p => p.AddressFamily == AddressFamily.InterNetwork);
                var localEndPoint = new IPEndPoint(ipAddress, 10001);

                Server.IServer server = new Server.Server(new Server.SocketConnection(localEndPoint, ProtocolType.Tcp, SocketType.Stream, AddressFamily.InterNetwork));
                server.Start();
            }
            else if (key.Key == ConsoleKey.C)
            {
                Client.IClient client = new Client.Client();
                while (true)
                {
                    Console.WriteLine("Enter command:");
                    var input = Console.ReadLine();
                    client.SendCommand(input);
                }
            }
            else
            {
                Console.WriteLine("Invalid character was entered");
            }
        }
    }
}

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
            var hostName = Dns.GetHostName();
            var ipHostInfo = Dns.GetHostEntry(hostName);
            var ipAddress = ipHostInfo.AddressList.LastOrDefault(p => p.AddressFamily == AddressFamily.InterNetwork);
            var localEndPoint = new IPEndPoint(ipAddress, 10001);

            IServer server = new Server(new SocketConnection(localEndPoint, ProtocolType.Tcp, SocketType.Stream, AddressFamily.InterNetwork));
            server.Start();

            
            //var listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //try
            //{
            //    listener.Bind(localEndPoint);
            //    listener.Listen(10);
            //    while (true)
            //    {
            //        Console.WriteLine("Start listening");
            //        var handler = listener.Accept();
            //        var data = string.Empty;
            //        while (true)
            //        {
            //            var buffer = new byte[256];
            //            var lengthRecData = handler.Receive(buffer);
            //            data += Encoding.ASCII.GetString(buffer, 0, lengthRecData);
            //            if (data.Contains(EndCommandRow))
            //                break;
            //        }

            //        var match = Regex.Match(data, string.Format(@"^{0}{1}$", "TIME", EndCommandRow), RegexOptions.IgnoreCase);
            //        //switch (data)
            //        //{
            //        //    case "":

            //        //}
            //    }
            //}
            //catch (Exception)
            //{
            //}
        }
    }
}

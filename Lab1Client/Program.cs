using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Lab1Client
{
    class Program
    {
        public readonly static List<string> _patternCollection = new List<string>
        {
            @"^(?<command>\w*)$",
            @"^(?<command>\w*)\s+'(?<param1>.*):(?<param2>.*)'$",
        };

        private static Match GetMatchByPatterns(string inputString)
        {
            foreach (var pattern in _patternCollection)
            {
                var match = Regex.Match(inputString, pattern, RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    return match;
                }
            }

            return null;
        }

        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("Enter command:");
                var input = Console.ReadLine();
                var match = GetMatchByPatterns(input);

                var command = match != null
                    ? match.Groups["command"].Value.ToUpper()
                    : "NULL";
                switch (command)
                {
                    case "CONNECT":
                        var group1 = match.Groups["param1"];
                        var group2 = match.Groups["param2"];
                        if (group1.Success && group2.Success)
                        {
                            var addressStr = group1.Value;
                            var portStr = group2.Value;
                            IPAddress ip;
                            var port = 0;
                            if (IPAddress.TryParse(addressStr, out ip) && int.TryParse(portStr, out port))
                            {
                                var client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                                client.Connect(ip, port);
                                var fileName = @"d:\ttt.txt";
                                client.SendFile(fileName);
                                client.Shutdown(SocketShutdown.Both);
                                client.Close();
                            }
                            else
                                Console.WriteLine("Address or port not valid");
                        }
                        else
                            Console.WriteLine("Command without parameters");

                        break;
                    case "QUIT":
                        return;
                    default:
                        Console.WriteLine("Command \"{0}\" is unknow", command);
                        break;
                }
            }
        }
    }
}

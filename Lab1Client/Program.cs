using System;
using System.Collections.Generic;
using System.IO;
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
        static void Main(string[] args)
        {
            Console.Title = "Client";
            IClient client = new Client();
            while (true)
            {
                Console.WriteLine("Enter command:");
                var input = Console.ReadLine();
                client.SendCommand(input);
            }
        }
    }
}

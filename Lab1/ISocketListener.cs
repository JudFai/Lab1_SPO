using System;
using System.Net.Sockets;

namespace Lab1
{
    class SocketDataEventArgs : EventArgs
    {
        public Socket Handler { get; private set; }
        public string Data { get; private set; }

        public SocketDataEventArgs(Socket handler, string data)
        {
            Handler = handler;
            Data = data;
        }
    }

    interface ISocketListener : IDisposable
    {
        event EventHandler<SocketDataEventArgs> DataReceived;
        void Start();
    }
}
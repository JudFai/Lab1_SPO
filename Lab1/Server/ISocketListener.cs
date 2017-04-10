using System;
using System.Net.Sockets;

namespace Lab1.Server
{
    class SocketEventArgs : EventArgs
    {
        public Socket Handler { get; private set; }

        public SocketEventArgs(Socket handler)
        {
            Handler = handler;
        }
    }

    class SocketDataEventArgs : SocketEventArgs
    {
        public byte[] Data { get; private set; }

        public SocketDataEventArgs(Socket handler, byte[] data)
            : base(handler)
        {
            Data = data;
        }
    }

    interface ISocketListener : IDisposable
    {
        event EventHandler<SocketDataEventArgs> DataReceived;
        event EventHandler<SocketEventArgs> ClientConnected;
        void Start();
    }
}
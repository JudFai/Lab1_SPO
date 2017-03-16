using System;
using System.Net;
using System.Net.Sockets;

namespace Lab1
{
    public interface IServer : IDisposable
    {
        ISocketConnection Connection { get; }
        void Start();
    }
}
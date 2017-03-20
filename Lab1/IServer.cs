using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Lab1
{
    public interface IServer : IDisposable
    {
        List<IClient> ConnectedClients { get; }
        ISocketConnection Connection { get; }
        void Start();
    }
}
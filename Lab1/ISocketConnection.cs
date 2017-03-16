﻿using System.Net;
using System.Net.Sockets;

namespace Lab1
{
    public interface ISocketConnection
    {
        IPEndPoint Address { get; }
        ProtocolType ProtocolType { get; }
        SocketType SocketType { get; }
        AddressFamily AddressFamily { get; }
    }
}
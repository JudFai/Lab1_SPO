using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Lab1
{
    public class SocketConnection : ISocketConnection
    {
        #region Constructors

        public SocketConnection(IPEndPoint addr, 
            ProtocolType protocolType, 
            SocketType socketType, 
            AddressFamily addressFamily)
        {
            Address = addr;
            ProtocolType = protocolType;
            SocketType = socketType;
            AddressFamily = addressFamily;
        }

        #endregion

        #region ISocketConnection Members

        public IPEndPoint Address { get; private set; }
        public ProtocolType ProtocolType { get; private set; }
        public SocketType SocketType { get; private set; }
        public AddressFamily AddressFamily { get; private set; }

        #endregion
    }
}

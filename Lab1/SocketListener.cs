﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace Lab1
{
    class SocketListener : ISocketListener
    {
        #region Fields

        private readonly Socket _listener;
        private readonly ISocketConnection _connection;
        private readonly IDataConverter _dataConverter;
        private bool _listeningWasStopped;

        #endregion

        #region Constructors

        public SocketListener(ISocketConnection connection, IDataConverter dataConverter)
        {
            _dataConverter = dataConverter;
            _connection = connection;
            _listener = new Socket(
                _connection.AddressFamily, 
                _connection.SocketType, 
                _connection.ProtocolType);
        }

        #endregion

        #region ISocketListener Members

        private void OnDataReceived(SocketDataEventArgs e)
        {
            if (DataReceived != null)
                DataReceived(this, e);
        }

        public event EventHandler<SocketDataEventArgs> DataReceived;
        public void Start(string dataEnd)
        {
            _listener.Bind(_connection.Address);
            _listener.Listen(10);
            Console.WriteLine("Start listening");
            while (true)
            {
                if (_listeningWasStopped)
                    return;

                var handler = _listener.Accept();
                var data = string.Empty;
                while (true)
                {
                    var buffer = new byte[256];
                    var lengthRecData = handler.Receive(buffer);
                    data += _dataConverter.GetString(buffer.Take(lengthRecData).ToArray());
                    if (data.Contains(dataEnd))
                        break;
                }

                OnDataReceived(new SocketDataEventArgs(handler, data));
            }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            // TODO: Dispose, Disconnect или то и другое
            DataReceived = null;
            _listeningWasStopped = true;
            _listener.Dispose();
            //_listener.Disconnect(true);
        }

        #endregion
    }
}
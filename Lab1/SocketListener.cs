using System;
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
        private string _fileExtension;

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

        #region Private Methods

        private void ReceivingData(Socket handler, string dataEnd)
        {
            var data = string.Empty;
            var timeout = DateTime.Now.AddSeconds(10);
            var wasTimeout = false;
            while (true)
            {
                if (timeout < DateTime.Now)
                {
                    wasTimeout = true;
                    break;
                }

                var buffer = new byte[256];
                var lengthRecData = handler.Receive(buffer);
                data += _dataConverter.GetString(buffer.Take(lengthRecData).ToArray());
                if (data.Contains(dataEnd))
                    break;
            }

            // Если был таймаут, то уничтожаем ресурсы полученного сокета и продолжаем сканирование
            if (wasTimeout)
            {
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
                return;
            }

            Console.WriteLine("RECEIVED DATA FROM <{0}>: {1}", handler.RemoteEndPoint, data.Replace(Environment.NewLine, string.Empty));

            OnDataReceived(new SocketDataEventArgs(handler, data));
        }

        private void ReceivingFile(Socket handler)
        {
            while (true)
            {
                // 1 Gbyte
                var buffer = new byte[1048576];
                var lengthRecData = handler.Receive(buffer);
                //data += _dataConverter.GetString(buffer.Take(lengthRecData).ToArray());
                //if (data.Contains(dataEnd))
                //    break;
            }
        }

        #endregion

        #region ISocketListener Members

        private void OnDataReceived(SocketDataEventArgs e)
        {
            if (DataReceived != null)
                DataReceived(this, e);
        }

        public bool ReceivingFileMode { get; private set; }
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
                if (ReceivingFileMode)
                    ReceivingFile(handler);
                else
                    ReceivingData(handler, dataEnd);
            }
        }

        public void ChangeModeToReceivingFile(string extension)
        {
            ReceivingFileMode = true;
            _fileExtension = extension;
        }

        public void ChangeModeToReceivingData()
        {
            ReceivingFileMode = false;
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            // TODO: Dispose, Disconnect или то и другое
            DataReceived = null;
            _listeningWasStopped = true;
            _listener.Dispose();
        }

        #endregion
    }
}

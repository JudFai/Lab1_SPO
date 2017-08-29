using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;

namespace Lab1.Server
{
    class SocketListener : ISocketListener
    {
        #region Fields

        private Socket _listener;
        private readonly ISocketConnection _connection;
        //private readonly IDataConverter _dataConverter;
        private Socket _currentClient;

        #endregion

        #region Constructors

        public SocketListener(ISocketConnection connection/*, IDataConverter dataConverter*/)
        {
            //_dataConverter = dataConverter;
            _connection = connection;
            _listener = new Socket(
                _connection.AddressFamily, 
                _connection.SocketType, 
                _connection.ProtocolType);
        }

        #endregion

        #region Private Methods

        private void ReceivingData()
        {
            //var data = string.Empty;
            //var timeout = DateTime.Now.AddSeconds(10);
            //var wasTimeout = false;
            //while (true)
            //{
                //if (timeout < DateTime.Now)
                //{
                //    wasTimeout = true;
                //    break;
                //}

            while (true)
            {
                try
                {
                    var data = ReceiveAllData(_currentClient);
                    if (data == null)
                        throw new SocketException();

                    OnDataReceived(new SocketDataEventArgs(_currentClient, data));
                }
                catch
                {
                    _currentClient = null;
                    break;
                }
            }
                //data += _dataConverter.GetString(buffer.Take(lengthRecData).ToArray());
                //if (data.Contains(dataEnd))
                //    break;
            //}

            // Если был таймаут, то уничтожаем ресурсы полученного сокета и продолжаем сканирование
            //if (wasTimeout)
            //{
            //    handler.Shutdown(SocketShutdown.Both);
            //    handler.Close();
            //    return;
            //}

            //Console.WriteLine("RECEIVED DATA FROM <{0}>: {1}", handler.RemoteEndPoint, data.Replace(Environment.NewLine, string.Empty));
        }

        private byte[] ReceiveAllData(Socket client)
        {
            var byteCollection = new List<byte>();
            client.ReceiveTimeout = 50;
            var receivedLength = 0;
            while (true)
            {
                try
                {
                    var receivedData = new byte[1048576];
                    receivedLength = client.Receive(receivedData);
                    if (receivedLength == 0)
                        return null;

                    byteCollection.AddRange(receivedData.Take(receivedLength).ToArray());
                }
                catch (SocketException ex)
                {
                    if (ex.ErrorCode == 10054)
                        return null;
                    else if (byteCollection.Count > 0)
                    {
                        break;
                    }
                }
                catch
                {
                    if (byteCollection.Count > 0)
                        break;
                }
            }

            return byteCollection.ToArray();
        }

        //private void ReceivingFile(Socket handler)
        //{
        //    var timeout = DateTime.Now.AddSeconds(30);
        //    var byteCollection = new List<byte>();
        //    //var path = Path.ChangeExtension(Path.Combine("Upload", DateTime.Now.Ticks.ToString()), _fileExtension);
        //    var lengthRecData = -1;
        //    while (true)
        //    {
        //        var buffer = new byte[1048576];
        //        lengthRecData = handler.Receive(buffer, buffer.Length, SocketFlags.None);
        //        if (lengthRecData > 0)
        //        {
        //            timeout = DateTime.Now.AddSeconds(30);
        //            byteCollection.AddRange(buffer);
        //        }
        //        else
        //        {
        //            if (timeout < DateTime.Now)
        //                throw new TimeoutException("ReceivingFile");
        //        }
                
        //        break;
        //    }

        //    var pathToDirectory = Path.GetDirectoryName(path);
        //    if (!Directory.Exists(pathToDirectory))
        //        Directory.CreateDirectory(pathToDirectory);

        //    File.WriteAllBytes(path, byteCollection.Take(lengthRecData).ToArray());
        //}

        #endregion

        #region ISocketListener Members

        private void OnDataReceived(SocketDataEventArgs e)
        {
            if (DataReceived != null)
                DataReceived(this, e);
        }

        private void OnClientConnected(SocketEventArgs e)
        {
            if (ClientConnected != null)
                ClientConnected(this, e);
        }

        public event EventHandler<SocketDataEventArgs> DataReceived;
        public event EventHandler<SocketEventArgs> ClientConnected;

        public void Start()
        {
            _listener.Bind(_connection.Address);
            _listener.Listen(10);
            Console.WriteLine("Start listening");
            while (true)
            {
                if (_listener == null)
                    break;
                
                _currentClient = _listener.Accept();
                OnClientConnected(new SocketEventArgs(_currentClient));
                ReceivingData();
            }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            // TODO: Dispose, Disconnect или то и другое
            DataReceived = null;
            _currentClient.Dispose();
            _listener.Dispose();
            _currentClient = null;
            _listener = null;
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Lab1
{
    public class Server : IServer
    {
        #region Fields

        private readonly ISocketListener _listener;
        private readonly IDataConverter _dataConverter;

        #endregion

        #region Constructors

        public Server(ISocketConnection connection)
        {
            Connection = connection;

            _dataConverter = new DataConverter(Encoding.ASCII);

            _listener = new SocketListener(connection, _dataConverter);
            _listener.DataReceived += OnDataReceived;
        }

        #endregion

        #region Private Methods

        private void OnDataReceived(object sender, SocketDataEventArgs e)
        {
            var handler = e.Handler;
            // TODO: сформировать данные, которые необходимо отправить клиенту
            var dataToClient = "OKAY" + Environment.NewLine;
            var bytes = _dataConverter.GetBytes(dataToClient);
            handler.Send(bytes);
            handler.Shutdown(SocketShutdown.Both);
            handler.Close();
        }

        #endregion

        #region IServer Members

        public ISocketConnection Connection { get; private set; }

        public void Start()
        {
            try
            {
                _listener.Start();
            }
            catch (Exception)
            {
                
                throw;
            }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            _listener.Dispose();
        }

        #endregion
    }
}

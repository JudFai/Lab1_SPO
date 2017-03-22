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

        private readonly IMessageConverter _clientMessageToServerMessageConverter;
        private readonly IMessageManager _messageManager;
        private readonly IClientDataConverter _dataToClientMessageConverter;
        private readonly ISocketListener _listener;
        private readonly IDataConverter _dataConverter;

        #endregion

        #region Constructors

        public Server(ISocketConnection connection)
        {
            ConnectedClients = new List<IClient>();
            Connection = connection;

            _dataConverter = new DataConverter(Encoding.ASCII);

            _listener = new SocketListener(connection, _dataConverter);
            _listener.DataReceived += OnDataReceived;

            _clientMessageToServerMessageConverter = MessageConverter.Instance;
            _messageManager = MessageManager.Instance;
            _dataToClientMessageConverter =
                ClientDataConverter.GetInstance(_clientMessageToServerMessageConverter.MessageEnd);
        }

        #endregion

        #region Private Methods

        private void RemoveClientFromCollection(IClient client)
        {
            //ConnectedClients.Remove(client);
            //client.Dispose();
        }

        private IClient AddNotExistClientToCollection(Socket handler)
        {
            //handler.SendFile("", null, null, T)
            IClient client = new Client(handler, _dataConverter);
            var foundClient = ConnectedClients.FirstOrDefault(p => p.Equals(client));
            if (foundClient != null)
                client = foundClient;
            else
                client.SentMessage += OnSentMessage;

            ConnectedClients.Add(client);
            return client;
        }

        private void OnSentMessage(object sender, EventArgs e)
        {
            var client = sender as IClient;
            if (client != null)
                RemoveClientFromCollection(client);
        }

        private void OnDataReceived(object sender, SocketDataEventArgs e)
        {
            var handler = e.Handler;
            var client = AddNotExistClientToCollection(handler);
            var clientMessage = _dataToClientMessageConverter.ConvertDataToClientMessage(e.Data);
            var serverMessage = _messageManager.Interpret(_listener, clientMessage);
            client.SendMessage(serverMessage);
        }

        #endregion

        #region IServer Members

        public List<IClient> ConnectedClients { get; private set; }
        public ISocketConnection Connection { get; private set; }

        public void Start()
        {
            try
            {
                _listener.Start(_clientMessageToServerMessageConverter.MessageEnd);
            }
            // TODO: обработка исключений
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

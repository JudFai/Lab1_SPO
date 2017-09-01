using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace Lab1.Server
{
    public class Server : IServer
    {
        #region Fields

        private readonly IMessageConverter _clientMessageToServerMessageConverter;
        private readonly IMessageManager _messageManager;
        private readonly IClientDataConverter _dataToClientMessageConverter;
        private readonly ISocketListener _listener;
        private readonly IDataConverter _dataConverter;
        private readonly IFileManager _fileManager;

        //private bool _fileIsUploading;
        //private IUploadingFile _currentUploadingFile;

        #endregion

        #region Constructors

        public Server(ISocketConnection connection)
        {
            ConnectedClients = new List<IClient>();
            Connection = connection;

            _dataConverter = new DataConverter(Encoding.ASCII);

            _listener = new SocketListener(connection/*, _dataConverter*/);
            _listener.DataReceived += OnDataReceived;
            _listener.ClientConnected += OnClientConnected;

            _clientMessageToServerMessageConverter = MessageConverter.Instance;
            _fileManager = FileManager.GetInstance("Upload");
            _messageManager = MessageManager.GetInstance(_dataConverter, _fileManager);
            _dataToClientMessageConverter =
                ClientDataConverter.GetInstance(_clientMessageToServerMessageConverter.MessageEnd);

            //UploadingFiles = new List<IUploadingFile>();
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
            var address = handler.RemoteEndPoint.ToString();
            IClient client = new Client(handler, _dataConverter);
            var foundClient = ConnectedClients.FirstOrDefault(p => p.Address == address);
            if (foundClient != null)
                client = foundClient;
            else
            {
                ConnectedClients.Add(client);
                client.SentMessage += OnSentMessage;
            }

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
            Received(e.Handler, e.Data);
        }

        private void OnClientConnected(object sender, SocketEventArgs e)
        {
            Console.WriteLine("Client '{0}' connected to server", e.Handler.RemoteEndPoint);
            Console.WriteLine();
        }

        private void Received(Socket handler, byte[] data)
        {
            //if (_fileIsUploading)
            //{
            //    var client = AddNotExistClientToCollection(handler);
            //    _currentUploadingFile.CurrentBytes.AddRange(data);
            //    var percents = _currentUploadingFile.GetLoadingPercentage();
            //    Console.WriteLine("Percents: {0}%", percents);
            //    client.SendMessage("OK");
            //    if (_currentUploadingFile.CurrentBytes.Count >= _currentUploadingFile.Size)
            //    {
            //        _fileManager.SaveFile(_currentUploadingFile);
            //        UploadingFiles.Remove(_currentUploadingFile);
            //        _fileIsUploading = false;
            //    }
            //}
            //else
            //{
                var client = AddNotExistClientToCollection(handler);
                var dataStr = _dataConverter.GetString(data);
                var clientMessage = _dataToClientMessageConverter.ConvertDataToClientMessage(dataStr);
                var serverMessage = _messageManager.Interpret(this, client, clientMessage);
                client.SendMessage(serverMessage);
            //}
        }

        #endregion

        #region IServer Members

        public List<IClient> ConnectedClients { get; private set; }
        //public List<IUploadingFile> UploadingFiles { get; private set; }
        public ISocketConnection Connection { get; private set; }

        public void Start()
        {
            try
            {
                _fileManager.ClearDirectory();
                _listener.Start();
            }
            // TODO: обработка исключений
            catch (Exception)
            {
            }
        }

        //public bool ChangeReceivingModeFile(IUploadingFile uploadingFile)
        //{
        //    //if (_fileIsUploading)
        //    //    return false;

        //    //if (UploadingFiles.Contains(uploadingFile))
        //    //    UploadingFiles.Add(uploadingFile);

        //    //_fileIsUploading = true;
        //    //_currentUploadingFile = uploadingFile;
        //    return true;
        //}

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            _listener.Dispose();
        }

        #endregion
    }
}

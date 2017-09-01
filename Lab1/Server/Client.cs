using System;
using System.Net.Sockets;

namespace Lab1.Server
{
    class Client : IClient
    {
        #region Fields

        private readonly IDataConverter _dataConverter;

        #endregion

        #region Properties

        public Socket Handler { get; private set; }

        #endregion

        #region Constructors

        public Client(Socket handler, IDataConverter dataConverter)
        {
            Handler = handler;
            Address = handler.RemoteEndPoint.ToString();
            _dataConverter = dataConverter;
        }

        #endregion

        #region Public Methods

        public override bool Equals(object obj)
        {
            return Equals(obj as IClient);
        }

        #endregion

        #region IClient Members

        private void OnSentMessage()
        {
            if (SentMessage != null)
                SentMessage(this, EventArgs.Empty);
        }

        public string Address { get; private set; }
        public event EventHandler SentMessage;

        public void SendMessage(IMessage message)
        {
            var bytes = _dataConverter.GetBytes(message.Data);
            Console.WriteLine("SEND DATA TO <{0}>: {1}", 
                Handler.RemoteEndPoint, message.Data.Length < 1000 
                ? message.Data.Replace(Environment.NewLine, string.Empty)
                : string.Format("{0} Lengths ", message.Data.Length));
            Console.WriteLine();
            var length = Handler.Send(bytes);
            OnSentMessage();
        }

        public void SendMessage(string message)
        {
            var bytes = _dataConverter.GetBytes(message);
            Console.WriteLine("SEND DATA TO <{0}>: {1}", Handler.RemoteEndPoint, message.Replace(Environment.NewLine, string.Empty));
            Console.WriteLine();
            var length = Handler.Send(bytes);
            OnSentMessage();
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            Handler.Shutdown(SocketShutdown.Both);
            Handler.Close();
            Handler.Dispose();
            SentMessage = null;
        }

        #endregion

        #region IEquatable Members

        public bool Equals(IClient other)
        {
            var instance = other as Client;
            if (instance == null)
                return false;

            return Handler.RemoteEndPoint.ToString() == instance.Handler.RemoteEndPoint.ToString();
        }

        #endregion
    }
}

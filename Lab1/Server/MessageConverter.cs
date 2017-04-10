using System;

namespace Lab1.Server
{
    class MessageConverter : IMessageConverter
    {
        #region Fields

        private static readonly object _instanceLocker = new object();
        private static IMessageConverter _instance;

        #endregion

        #region Properties

        public static IMessageConverter Instance
        {
            get
            {
                lock (_instanceLocker)
                {
                    return _instance ??
                           (_instance = new MessageConverter(Environment.NewLine));
                }
            }
        }

        #endregion

        #region Constructors

        private MessageConverter(string messageEnd)
        {
            MessageEnd = messageEnd;
        }

        #endregion

        #region IMessageConverter Members

        public string MessageEnd { get; private set; }

        public ServerMessage Convert(string data, ClientCommand clientCommand)
        {
            data += MessageEnd;

            return new ServerMessage(data, clientCommand);
        }

        #endregion
    }
}

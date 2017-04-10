using System.Collections.Generic;

namespace Lab1.Server
{
    class ClientMessage : IMessage
    {
        #region Properties

        public List<object> CommandParameters { get; private set; }

        #endregion

        #region Constructors

        public ClientMessage(string data, ClientCommand clientCommand, List<object> commandParameters = null)
        {
            Data = data;
            ClientCommand = clientCommand;
            CommandParameters = commandParameters;
        }

        #endregion

        #region IMessage Members

        public string Data { get; private set; }
        public ClientCommand ClientCommand { get; private set; }

        #endregion
    }
}

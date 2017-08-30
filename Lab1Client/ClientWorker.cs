using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1Client
{
    public class ClientWorker : IClientWorker
    {
        #region Fields

        private readonly ICommandClientConverter _converter = new CommandClientConverter();

        #endregion

        #region Constructors

        public ClientWorker(IClient client)
        {
            Client = client;
        }

        #endregion

        #region IClientWorker Members

        public IClient Client { get; private set; }

        public void Send(CommandClient command, params object[] param)
        {
            var cmd = _converter.ConvertToCommand(command, param);
            Client.SendCommand(cmd);
        }

        #endregion
    }
}

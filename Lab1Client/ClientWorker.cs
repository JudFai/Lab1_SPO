using System;
using System.Collections.Generic;
using System.Globalization;
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

        //public ClientWorker(IClient client)
        //{
        //    Client = client;
        //}

        public ClientWorker()
        {
            Client = new Client();
        }

        #endregion

        #region IClientWorker Members

        public IClient Client { get; private set; }

        public object Send(CommandClient command, params object[] param)
        {
            var cmd = _converter.ConvertToCommand(command, param);
            var receiveMessage = Client.SendCommand(cmd);
            object receive = null;
            switch (command)
            {
                case CommandClient.Time:
                    if (receiveMessage != null)
                        receive = DateTime.ParseExact(
                            (string)receiveMessage, 
                            string.Format("{0}{1}", "dd.MM.yyyy HH:mm:ss", Environment.NewLine), 
                            CultureInfo.InvariantCulture);
                    break;
                case CommandClient.Echo:
                    if (receiveMessage != null)
                        receive = ((string) receiveMessage).Replace(Environment.NewLine, string.Empty);
                    break;
                default:
                    return receiveMessage;
            }

            return receive;
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            Client.Dispose();
        }

        #endregion

    }
}

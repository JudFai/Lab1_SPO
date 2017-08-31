using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Lab1Client
{
    public class CommandClientConverter : ICommandClientConverter
    {
        #region ICommandClientConverter Members

        public string ConvertToCommand(CommandClient commandClient, object[] param)
        {
            var command = commandClient.ToString().ToUpper();
            switch (commandClient)
            {
                case CommandClient.Connect:
                    var ip = (IPEndPoint)param[0];
                    command = string.Format("{0} '{1}'", command, ip);
                    break;
                case CommandClient.Upload:
                case CommandClient.Echo:
                case CommandClient.Donwload:
                    var str = (string)param[0];
                    command = string.Format("{0} '{1}'", command, str);
                    break;
                case CommandClient.Time:
                case CommandClient.Close:
                    break;
                default:
                    throw new NotImplementedException();
            }

            return command;
        }

        #endregion

    }
}

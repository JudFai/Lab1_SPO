using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1Client
{
    public interface IClientWorker
    {
        IClient Client { get; }
        void Send(CommandClient command, params object[] param);
    }
}

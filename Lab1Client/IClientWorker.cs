using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1Client
{
    public interface IClientWorker : IDisposable
    {
        IClient Client { get; }
        object Send(CommandClient command, params object[] param);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1Client
{
    public interface ICommandClientConverter
    {
        string ConvertToCommand(CommandClient command, object[] param);
    }
}

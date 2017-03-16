using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lab1
{
    interface IDataConverter
    {
        byte[] GetBytes(string str);
        string GetString(byte[] data);
    }
}

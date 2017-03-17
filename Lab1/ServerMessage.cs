using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lab1
{
    class ServerMessage : IMessage
    {
        #region Constructors

        public ServerMessage(string data)
        {
            Data = data;
        }

        #endregion

        #region IMessage Members

        public string Data { get; private set; }

        #endregion
    }
}

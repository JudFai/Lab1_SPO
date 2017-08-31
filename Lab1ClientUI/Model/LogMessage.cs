using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lab1ClientUI.Model
{
    public class LogMessage
    {
        #region Properties

        public string Message { get; private set; }
        public DateTime Time { get; private set; }

        #endregion


        #region Constructors

        public LogMessage(DateTime time, string message)
        {
            Time = time;
            Message = message;
        }

        #endregion

        public override string ToString()
        {
            return string.Format("[{0:HH:mm:ss}] {1}", Time, Message);
        }
    }
}

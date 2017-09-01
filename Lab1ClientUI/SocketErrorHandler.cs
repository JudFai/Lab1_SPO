using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace Lab1ClientUI
{
    class SocketErrorHandler : ISocketErrorHandler
    {
        #region ISocketErrorHandler Members

        public event EventHandler SocketWasAborded;

        private void OnSocketWasAborded()
        {
            if (SocketWasAborded != null)
                SocketWasAborded(this, EventArgs.Empty);
        }

        public void Handle(Action act)
        {
            try
            {
                act.Invoke();
            }
            catch (SocketException e)
            {
                switch (e.SocketErrorCode)
                {
                    case SocketError.ConnectionReset:
                    case SocketError.Disconnecting:
                        OnSocketWasAborded();
                        break;
                }
            }
        }

        #endregion

    }
}

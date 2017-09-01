using System;

namespace Lab1ClientUI
{
    public interface ISocketErrorHandler
    {
        event EventHandler SocketWasAborded;
        void Handle(Action act);
    }
}

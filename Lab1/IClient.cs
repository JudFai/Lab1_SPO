using System;

namespace Lab1
{
    public interface IClient : IDisposable, IEquatable<IClient>
    {
        event EventHandler SentMessage; 
        void SendMessage(IMessage message);
    }
}
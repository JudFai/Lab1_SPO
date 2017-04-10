using System;

namespace Lab1.Server
{
    public interface IClient : IDisposable, IEquatable<IClient>
    {
        string Address { get; } 
        event EventHandler SentMessage; 
        void SendMessage(IMessage message);
        void SendMessage(string message);
    }
}
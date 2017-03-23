using System;
using System.Net;

namespace Lab1
{
    public interface IClient : IDisposable, IEquatable<IClient>
    {
        string Address { get; } 
        event EventHandler SentMessage; 
        void SendMessage(IMessage message);
    }
}
using System;

namespace Lab1Client
{
    public class ProgressEventArgs : EventArgs
    {
        public int Progress { get; private set; }

        public ProgressEventArgs(int progress)
        {
            Progress = progress;
        }
    }

    public class MessageEventArgs : EventArgs
    {
        public string Message { get; private set; }
        public DateTime Time { get; private set; }

        public MessageEventArgs(DateTime time, string message)
        {
            Time = time;
            Message = message;
        }
    }

    public interface IClient : IDisposable
    {
        event EventHandler<ProgressEventArgs> ProgressChanged;
        event EventHandler<MessageEventArgs> MessageReceived;
        int CurrentProgress { get; }
        object SendCommand(string cmd);
    }
}

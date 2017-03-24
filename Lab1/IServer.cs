using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Lab1
{
    public interface IServer : IDisposable
    {
        List<IClient> ConnectedClients { get; }
        List<IUploadingFile> UploadingFiles { get; }
        ISocketConnection Connection { get; }
        void Start();
        void ChangeReceivingModeFile(IUploadingFile uploadingFile);
    }
}
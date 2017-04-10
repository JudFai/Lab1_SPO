using System;
using System.Collections.Generic;

namespace Lab1.Server
{
    public interface IServer : IDisposable
    {
        List<IClient> ConnectedClients { get; }
        //List<IUploadingFile> UploadingFiles { get; }
        ISocketConnection Connection { get; }
        void Start();
        //bool ChangeReceivingModeFile(IUploadingFile uploadingFile);
        void WaitFileData();
    }
}
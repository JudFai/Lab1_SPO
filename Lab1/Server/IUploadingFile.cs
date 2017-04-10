using System;
using System.Collections.Generic;

namespace Lab1.Server
{
    public interface IUploadingFile : IEquatable<IUploadingFile>
    {
        string Path { get; }
        IClient Client { get; }
        int Size { get; }
        List<byte> CurrentBytes { get; }
        int GetLoadingPercentage();
    }
}

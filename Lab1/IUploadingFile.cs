using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lab1
{
    public interface IUploadingFile : IEquatable<IUploadingFile>
    {
        string Path { get; }
        IClient Client { get; }
        int Size { get; }
        List<byte> CurrentBytes { get; }
    }
}

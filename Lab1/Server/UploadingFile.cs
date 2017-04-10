using System;
using System.Collections.Generic;

namespace Lab1.Server
{
    class UploadingFile : IUploadingFile
    {
        #region Constructors

        public UploadingFile(string path, IClient client, int size, List<byte> currentBytes)
        {
            Path = path;
            Client = client;
            Size = size;
            CurrentBytes = currentBytes;
        }

        public UploadingFile(string path, IClient client, int size)
            : this(path, client, size, new List<byte>())
        { }

        #endregion

        #region IUploadingFile Members

        public string Path { get; private set; }
        public IClient Client { get; private set; }
        public int Size { get; private set; }
        public List<byte> CurrentBytes { get; private set; }

        public int GetLoadingPercentage()
        {
            var percents = Convert.ToInt32(Math.Round((CurrentBytes.Count / (double)Size) * 100));
            return percents > 100 ? 100 : percents;
        }

        #endregion

        #region IEquatable Members

        public bool Equals(IUploadingFile other)
        {
            if (other == null)
                return false;

            return Path == other.Path && Client.Equals(other.Client);
        }

        #endregion
    }
}

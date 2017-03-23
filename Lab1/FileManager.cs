using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Lab1
{
    class FileManager : IFileManager
    {
        #region Fields

        private static readonly object _instanceLocker = new object();
        private static IFileManager _instance;

        #endregion

        #region Constructors

        private FileManager(string pathToDirectory)
        {
            PathToDirectory = pathToDirectory;
        }

        #endregion

        #region Public Methods

        public static IFileManager GetInstance(string pathToDirectory)
        {
            lock (_instanceLocker)
            {
                return _instance ??
                       (_instance = new FileManager(pathToDirectory));
            }
        }

        #endregion

        #region IFileManager Members

        public string PathToDirectory { get; private set; }
        public void SaveFile(IUploadingFile uploadingFile)
        {
            throw new NotImplementedException();
        }

        public void ClearDirectory()
        {
            if (Directory.Exists(PathToDirectory))
            {
                var di = new DirectoryInfo(PathToDirectory);
                foreach (var file in di.GetFiles())
                    file.Delete();
            }
            else
                Directory.CreateDirectory(PathToDirectory);
        }

        #endregion
    }
}

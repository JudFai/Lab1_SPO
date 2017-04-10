namespace Lab1.Server
{
    interface IFileManager
    {
        string PathToDirectory { get; }
        void SaveFile(IUploadingFile uploadingFile);
        void ClearDirectory();
    }
}

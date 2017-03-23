namespace Lab1
{
    interface IFileManager
    {
        string PathToDirectory { get; }
        void SaveFile(IUploadingFile uploadingFile);
        void ClearDirectory();
    }
}

namespace Lab1.Server
{
    public enum ClientCommand 
    {
        Error = 0,
        Echo = 1,
        Time = 2,
        Close = 3,
        BeginUpload = 4,
        ContinueUpload = 5,
        FinishUpload = 6,
        Download = 7
    }
}

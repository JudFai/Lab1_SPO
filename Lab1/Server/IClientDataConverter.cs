namespace Lab1.Server
{
    interface IClientDataConverter
    {
        ClientMessage ConvertDataToClientMessage(string data);
    }
}

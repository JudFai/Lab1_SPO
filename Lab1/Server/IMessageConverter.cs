namespace Lab1.Server
{
    interface IMessageConverter
    {
        string MessageEnd { get; }
        ServerMessage Convert(string data, ClientCommand clientCommand);
    }
}

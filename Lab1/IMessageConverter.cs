namespace Lab1
{
    interface IMessageConverter
    {
        string MessageEnd { get; }
        ServerMessage Convert(string data, ClientCommand clientCommand);
    }
}

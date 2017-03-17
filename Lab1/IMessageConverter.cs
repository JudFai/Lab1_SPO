namespace Lab1
{
    interface IMessageConverter
    {
        string MessageEnd { get; }
        ServerMessage Convert(ClientMessage clientMessage);
    }
}

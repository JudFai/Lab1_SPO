namespace Lab1
{
    interface IMessageManager
    {
        ServerMessage Interpret(ISocketListener listener, IClient client, ClientMessage clientMessage);
    }
}
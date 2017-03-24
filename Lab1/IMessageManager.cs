namespace Lab1
{
    interface IMessageManager
    {
        ServerMessage Interpret(IServer server, IClient client, ClientMessage clientMessage);
    }
}
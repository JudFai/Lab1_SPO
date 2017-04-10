namespace Lab1.Server
{
    interface IMessageManager
    {
        ServerMessage Interpret(IServer server, IClient client, ClientMessage clientMessage);
    }
}
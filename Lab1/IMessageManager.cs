namespace Lab1
{
    interface IMessageManager
    {
        ServerMessage Interpret(IServer server, ClientMessage clientMessage);
    }
}
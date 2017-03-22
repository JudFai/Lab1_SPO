namespace Lab1
{
    interface IMessageManager
    {
        ServerMessage Interpret(ISocketListener listener, ClientMessage clientMessage);
    }
}
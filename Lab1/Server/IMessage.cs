namespace Lab1.Server
{
    public interface IMessage
    {
        string Data { get; }
        ClientCommand ClientCommand { get; } 
    }
}

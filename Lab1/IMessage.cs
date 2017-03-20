namespace Lab1
{
    public interface IMessage
    {
        string Data { get; }
        ClientCommand ClientCommand { get; } 
    }
}

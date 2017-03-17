namespace Lab1
{
    interface IMessage
    {
        string Data { get; }
        ClientCommand ClientCommand { get; } 
    }
}

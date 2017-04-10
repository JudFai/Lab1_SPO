namespace Lab1.Server
{
    interface IDataConverter
    {
        byte[] GetBytes(string str);
        string GetString(byte[] data);
    }
}

namespace MessageReceiverAsAService.Lib.Interfaces
{
    public interface IBinarySerializer
    {
        byte[] Serialize<T>(T value);

        T Deserialize<T>(byte[] data) where T : class;
    }
}
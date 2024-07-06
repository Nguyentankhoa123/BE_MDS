namespace MDS.Services
{
    public interface IRedisService
    {
        Task<string> AcquireLockAsync(int productId, int quantity, int cartId);
    }
}

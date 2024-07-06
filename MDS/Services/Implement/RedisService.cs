
using MDS.Shared.Database.DbContext;
using Medallion.Threading.Redis;
using StackExchange.Redis;

namespace MDS.Services.Implement
{
    public class RedisService : IRedisService
    {
        private readonly IDatabase _redisDb;
        private readonly AppDbContext _cotext;
        private readonly IConnectionMultiplexer _redis;
        private readonly IInventoryService _inventoryService;
        public RedisService(AppDbContext context, IConnectionMultiplexer redis, IInventoryService inventoryService)
        {
            _cotext = context;
            _inventoryService = inventoryService;
            _redis = redis;
            _redisDb = redis.GetDatabase();

            var endpoints = _redisDb.Multiplexer.GetEndPoints();
            var server = _redisDb.Multiplexer.GetServer(endpoints.First());
            Console.WriteLine($"Connected to Redis server: {server.EndPoint}");
        }
        public async Task<string> AcquireLockAsync(int productId, int quantity, int cartId)
        {
            string key = $"lock_v2024_{productId}";
            int retryTimes = 10;
            TimeSpan expiry = TimeSpan.FromMilliseconds(3000);

            for (int i = 0; i < retryTimes; i++)
            {
                var @lock = new RedisDistributedLock(key, _redisDb);
                {
                    await using (var handle = await @lock.TryAcquireAsync())
                        if (handle != null)
                        {
                            int modifiedCount = await _inventoryService.ReservationInventory(productId, quantity, cartId);
                            if (modifiedCount > 0)
                            {
                                return key;
                            }
                        }
                        else
                        {
                            Console.WriteLine($"Unable to acquire lock for product {productId}. Retrying...");
                            await Task.Delay(50);
                        }
                }
            }
            Console.WriteLine($"Failed to acquire lock for product {productId} after {retryTimes} attempts.");
            return null;
        }
    }
}

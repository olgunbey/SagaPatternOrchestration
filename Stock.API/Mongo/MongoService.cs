using MongoDB.Driver;

namespace Stock.API.Mongo;

public class MongoService
{
    private readonly MongoClient _client;
    public MongoService()
    {
        _client = new MongoClient("mongodb+srv://olgunbey:ABaiMqiltPgq6qaT@parametrecluster.hvwzz.mongodb.net/");
    }

    public async Task<bool> StockControl(List<StockControl> stockControls)
    {
        var collection = _client.GetDatabase("ParametreCluster").GetCollection<Stock>("Stock");
        List<bool> resultStatus = new();
        foreach (var stockControl in stockControls)
        {

            var data = await collection.Find(y => y.ProductId == stockControl.ProductId).SingleOrDefaultAsync();
            resultStatus.Add(data.Count >= stockControl.Count);
        }
        return resultStatus.TrueForAll(y => y.Equals(true));
    }

    public async Task AddStock()
    {
        await _client.GetDatabase("ParametreCluster").GetCollection<Stock>("Stock")
             .InsertManyAsync(new List<Stock>()
             {
                new Stock()
                {
                    Id = 1,
                    ProductId = 1,
                    Count = 25
                },
                new Stock()
                {
                    Id = 2,
                    ProductId = 2,
                    Count = 50
                }
             });
    }

    public async Task InventoryReductionAsync(List<StockControl> stockControls)
    {
        var collection = _client.GetDatabase("StockControl").GetCollection<Stock>("Stock");
        foreach (var stockControl in stockControls)
        {
            var data = collection.FindAsync(y =>
                y.ProductId == stockControl.ProductId && y.Count >= stockControl.Count);

            var hasData = (await data).Any();
            if (hasData)
            {
                var singleData = (await data).SingleOrDefault();
                singleData.Count -= stockControl.Count;

                await collection.ReplaceOneAsync(y => y.ProductId == stockControl.ProductId, singleData);
            }

        }
    }
}

public class Stock
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int Count { get; set; }
}

public class StockControl
{
    public int ProductId { get; set; }
    public int Count { get; set; }
}
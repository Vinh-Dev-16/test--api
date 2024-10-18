using cloud.core;
using cloud.core.mongodb;
using MongoDB.Driver;

namespace testapi;


public class AdsMongoDbContext(string? connectionString) : BaseMongoObjectIdDbContext(connectionString)
{

    public IMongoCollection<CalculationResult> CalculationResults =>
        Database.GetCollection<CalculationResult>("calculation_results");
}

using cloud.core.mongodb;
using MongoDB.Bson;

namespace testapi;

public class CalculationResult
{
    public string? Id { get; set; } 
    public int Number { get; set; }
    public List<int>? Data { get; set; }
    public int Answer { get; set; } 
}
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using testapi.Model;

namespace testapi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class Bai212Controller : ControllerBase
{
    private readonly AdsMongoDbContext _context;

    public Bai212Controller(IConfiguration configuration, AdsMongoDbContext context)
    {
        _context = context;
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CalculationInputModel? input)
    {
        if (input == null || input.Data == null || !input.Data.Any())
        {
            return BadRequest(new { message = "Dữ liệu đầu vào không hợp lệ" });
        }

        var positiveNumbers = input.Data.Where(x => x > 0).ToList();
        double average = positiveNumbers.Any() ? positiveNumbers.Average() : 0;

        var result = new CalculationResult
        {
            Id = Guid.NewGuid().ToString(),
            Number = positiveNumbers.Count,
            Data = positiveNumbers,
            Answer = (int)average
        };

        await _context.CalculationResults.InsertOneAsync(result);

        return Ok(new { message = "Thêm dữ liệu thành công", result });
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var results = await _context.CalculationResults.Find(r => true).ToListAsync();
        return Ok(results);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var result = await _context.CalculationResults.Find(r => r.Id == id).FirstOrDefaultAsync();
        if (result == null)
        {
            return NotFound(new { message = "Không tìm thấy kết quả." });
        }
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] CalculationInputModel? input)
    {
        if (input == null || input.Data == null || !input.Data.Any())
        {
            return BadRequest(new { message = "Dữ liệu đầu vào không hợp lệ" });
        }

        var existingResult = await _context.CalculationResults.Find(r => r.Id == id).FirstOrDefaultAsync();
        if (existingResult == null)
        {
            return NotFound(new { message = "Không tìm thấy kết quả." });
        }

        var positiveNumbers = input.Data.Where(x => x > 0).ToList();
        double average = positiveNumbers.Any() ? positiveNumbers.Average() : 0;

        existingResult.Data = positiveNumbers;
        existingResult.Number = positiveNumbers.Count;
        existingResult.Answer = (int)average;

        await _context.CalculationResults.ReplaceOneAsync(r => r.Id == existingResult.Id, existingResult);

        return Ok(new { message = "Cập nhật thành công", result = existingResult });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var result = await _context.CalculationResults.Find(r => r.Id == id).FirstOrDefaultAsync();
        if (result == null)
        {
            return NotFound(new { message = "Không tìm thấy kết quả." });
        }

        await _context.CalculationResults.DeleteOneAsync(r => r.Id == result.Id);
        return Ok(new { message = "Xóa thành công" });
    }
}

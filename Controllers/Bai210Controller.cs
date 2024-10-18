using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using testapi.Model;

namespace testapi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class Bai210Controller : ControllerBase
{
    private readonly AdsMongoDbContext _context;

    public Bai210Controller(IConfiguration configuration, AdsMongoDbContext context)
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

        int sum = input.Data.Where(x => IsFirstDigitEven(x)).Sum();
        int number = input.Data.Count;

        var result = new CalculationResult
        {
            Id = Guid.NewGuid().ToString(), 
            Number = number,
            Data = input.Data,
            Answer = sum
        };

        await _context.CalculationResults.InsertOneAsync(result);

        return Ok(new { message = "Thêm dữ liệu thành công", result });
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var results = await _context.CalculationResults.Find(_ => true).ToListAsync();
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

        existingResult.Data = input.Data;
        existingResult.Number = input.Data.Count;
        existingResult.Answer = input.Data.Where(x => IsFirstDigitEven(x)).Sum();

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

    private bool IsFirstDigitEven(int number)
    {
        int firstDigit = Math.Abs(number);
        while (firstDigit >= 10)
        {
            firstDigit /= 10;
        }
        return firstDigit % 2 == 0;
    }
}



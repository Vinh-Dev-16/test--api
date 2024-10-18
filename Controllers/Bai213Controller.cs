using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using testapi.Model;

namespace testapi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class Bai213Controller : ControllerBase
{
    private readonly AdsMongoDbContext _context;

    public Bai213Controller(IConfiguration configuration, AdsMongoDbContext context)
    {
        _context = context;
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CalculationInput2Model? input)
    {
        if (input == null || input.Data == null)
        {
            return BadRequest(new { message = "Dữ liệu đầu vào không hợp lệ" });
        }

        var filteredData = input.Data.Where(value => value > input.X).ToList();
        double average = filteredData.Any() ? filteredData.Average() : 0;

        var result = new CalculationResult
        {
            Id = Guid.NewGuid().ToString(),
            Number = filteredData.Count,
            Data = filteredData,
            Answer = (int)average
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
            return NotFound(new { message = "Không tìm thấy kết quả" });
        }
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] CalculationResult updatedResult)
    {
        if (id != updatedResult.Id)
        {
            return BadRequest(new { message = "Id không hợp lệ" });
        }

        var result = await _context.CalculationResults.Find(r => r.Id == id).FirstOrDefaultAsync();
        if (result == null)
        {
            return NotFound(new { message = "Không tìm thấy kết quả" });
        }

        await _context.CalculationResults.ReplaceOneAsync(r => r.Id == id, updatedResult);
        return Ok(new { message = "Cập nhật thành công", updatedResult });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var result = await _context.CalculationResults.Find(r => r.Id == id).FirstOrDefaultAsync();
        if (result == null)
        {
            return NotFound(new { message = "Không tìm thấy kết quả" });
        }

        await _context.CalculationResults.DeleteOneAsync(r => r.Id == id);
        return Ok(new { message = "Xóa thành công" });
    }
}


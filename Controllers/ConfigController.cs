namespace A1_order_system.Controllers;

using A1_order_system.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/config")]
[Authorize(Roles = "Admin")]
public class ConfigController : ControllerBase
{
    private readonly ConfigService _service;

    public ConfigController(ConfigService service) => _service = service;

    [HttpGet("taxa-delivery")]
    public async Task<IActionResult> GetTaxaDelivery()
        => Ok(new { taxa = await _service.GetTaxaDeliveryProprioAsync() });

    [HttpPut("taxa-delivery")]
    public async Task<IActionResult> SetTaxaDelivery([FromBody] decimal taxa)
    {
        if (taxa < 0)
            return BadRequest(new { erro = "Taxa não pode ser negativa." });

        await _service.SetTaxaDeliveryProprioAsync(taxa);
        return Ok(new { taxa });
    }
}

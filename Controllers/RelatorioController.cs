namespace A1_order_system.Controllers;
using A1_order_system.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


[ApiController]
[Route("api/relatorios")]
[Authorize(Roles = "Admin")]
public class RelatorioController : ControllerBase
{
    private readonly RelatorioService _service;

    public RelatorioController(RelatorioService service) => _service = service;

    [HttpGet("faturamento")]
    public async Task<IActionResult> Faturamento([FromQuery] DateTime inicio, [FromQuery] DateTime fim)
        => Ok(await _service.FaturamentoPorTipoAsync(inicio, fim));

    [HttpGet("mais-vendidos")]
    public async Task<IActionResult> MaisVendidos([FromQuery] DateTime inicio, [FromQuery] DateTime fim)
        => Ok(await _service.ItensMaisVendidosAsync(inicio, fim));
}

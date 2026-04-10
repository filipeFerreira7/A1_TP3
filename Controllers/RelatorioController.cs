namespace A1_order_system.Controllers;
using A1_order_system.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


[ApiController]
[Route("api/relatorios")]
[Authorize]
public class RelatorioController : ControllerBase
{
    private readonly RelatorioService _service;

    public RelatorioController(RelatorioService service) => _service = service;

    /// <summary>
    /// Faturamento total agrupado por tipo de atendimento em um período.
    /// Ex.: GET /api/relatorios/faturamento?inicio=2025-01-01&amp;fim=2025-01-31
    /// </summary>
    [HttpGet("faturamento")]
    public async Task<IActionResult> Faturamento([FromQuery] DateTime inicio, [FromQuery] DateTime fim)
        => Ok(await _service.FaturamentoPorTipoAsync(inicio, fim));

    /// <summary>
    /// Itens mais vendidos no período, indicando quantas vezes foram vendidos como Sugestão do Chefe.
    /// Ex.: GET /api/relatorios/mais-vendidos?inicio=2025-01-01&amp;fim=2025-01-31
    /// </summary>
    [HttpGet("mais-vendidos")]
    public async Task<IActionResult> MaisVendidos([FromQuery] DateTime inicio, [FromQuery] DateTime fim)
        => Ok(await _service.ItensMaisVendidosAsync(inicio, fim));
}
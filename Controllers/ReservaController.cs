namespace A1_order_system.Controllers;

using A1_order_system.Dtos;
using A1_order_system.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/reservas")]
[Authorize]
public class ReservaController : BaseController
{
    private readonly ReservaService _service;

    public ReservaController(ReservaService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> Listar()
        => Ok(await _service.ListarReservasAsync(UsuarioIdLogado));

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] ReservaDto dto)
    {
        try
        {
            var result = await _service.CriarReservaAsync(dto, UsuarioIdLogado);
            return CreatedAtAction(nameof(Listar), new { id = result.Id }, result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { erro = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { erro = ex.Message });
        }
    }
}


namespace A1_order_system.Controllers;

using A1_order_system.Dtos;
using A1_order_system.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;



[ApiController]
[Route("api/pedidos")]
[Authorize]
public class PedidoController : BaseController
{
    private readonly PedidoService _service;

    public PedidoController(PedidoService service) => _service = service;

    /// <summary>Lista todos os pedidos do usuário autenticado.</summary>
    [HttpGet]
    public async Task<IActionResult> Listar()
        => Ok(await _service.ListarPedidosUsuarioAsync(UsuarioIdLogado));

    /// <summary>
    /// Cria um novo pedido.
    /// TipoAtendimento: "Presencial" | "DeliveryProprio" | "DeliveryApp"
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] PedidoDto dto)
    {
        try
        {
            var result = await _service.CriarPedidoAsync(dto, UsuarioIdLogado);
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
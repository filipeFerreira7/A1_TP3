namespace A1_order_system.Controllers;
using A1_order_system.Dtos;
using A1_order_system.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/enderecos")]
[Authorize]
public class EnderecoController : BaseController
{
    private readonly EnderecoService _service;

    public EnderecoController(EnderecoService service) => _service = service;

    /// <summary>Lista todos os endereços do usuário autenticado.</summary>
    [HttpGet]
    public async Task<IActionResult> Listar()
        => Ok(await _service.ListarAsync(UsuarioIdLogado));

    /// <summary>Adiciona um novo endereço de entrega.</summary>
    [HttpPost]
    public async Task<IActionResult> Adicionar([FromBody] EnderecoDto dto)
    {
        var result = await _service.AdicionarAsync(dto, UsuarioIdLogado);
        return CreatedAtAction(nameof(Listar), result);
    }

    /// <summary>Remove um endereço pelo ID.</summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Remover(long id)
    {
        try
        {
            await _service.RemoverAsync(id, UsuarioIdLogado);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { erro = ex.Message });
        }
    }
}
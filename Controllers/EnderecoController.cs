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

    [HttpGet]
    public async Task<IActionResult> Listar()
        => Ok(PerfilUsuarioLogado == "Admin"
            ? await _service.ListarTodosAsync()
            : await _service.ListarAsync(UsuarioIdLogado));

    [HttpPost]
    public async Task<IActionResult> Adicionar([FromBody] EnderecoDto dto)
    {
        var result = await _service.AdicionarAsync(dto, UsuarioIdLogado);
        return CreatedAtAction(nameof(Listar), result);
    }

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

namespace A1_order_system.Controllers;
using A1_order_system.Dtos;
using A1_order_system.Services;
using global::RestauranteApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/cardapio")]
public class CardapioController : BaseController
{
    private readonly CardapioService _cardapioService;
    private readonly SugestaoService _sugestaoService;

    public CardapioController(CardapioService cardapioService, SugestaoService sugestaoService)
    {
        _cardapioService = cardapioService;
        _sugestaoService = sugestaoService;
    }

    /// <summary>Lista todos os itens do cardápio, opcionalmente filtrando por período.</summary>
    [HttpGet]
    public async Task<IActionResult> Listar([FromQuery] Periodo? periodo)
        => Ok(await _cardapioService.ListarAsync(periodo));

    /// <summary>Retorna as Sugestões do Chefe do dia.</summary>
    [HttpGet("sugestoes-hoje")]
    public async Task<IActionResult> SugestoesHoje()
        => Ok(await _sugestaoService.SugestoesHojeAsync());

    /// <summary>Cria um novo item no cardápio. (Admin)</summary>
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CriarItem([FromBody] ItemCardapioDto dto)
    {
        try
        {
            var item = await _cardapioService.CriarItemAsync(dto);
            return CreatedAtAction(nameof(Listar), new { id = item.Id }, item);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { erro = ex.Message });
        }
    }

    /// <summary>Define a Sugestão do Chefe do dia para um período. (Admin)</summary>
    [HttpPost("sugestao")]
    [Authorize]
    public async Task<IActionResult> DefinirSugestao([FromBody] SugestaoDto dto)
    {
        try
        {
            var result = await _sugestaoService.DefinirSugestaoAsync(dto, UsuarioIdLogado);
            return Ok(result);
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
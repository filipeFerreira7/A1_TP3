using A1_order_system;
using A1_order_system.Data;
using A1_order_system.Dtos;
using A1_order_system.Entities;
using Microsoft.EntityFrameworkCore;

namespace RestauranteApp.Services;

public class CardapioService
{
    private readonly RestauranteDbContext _context;

    public CardapioService(RestauranteDbContext context) => _context = context;

    public async Task<List<ItemCardapioResponseDto>> ListarAsync(Periodo? periodo = null)
    {
        var hoje = DateTime.UtcNow.Date;

        var sugestoesHoje = await _context.SugestoesChefe
            .Where(s => s.Data.Date == hoje)
            .ToListAsync();

        var query = _context.ItensCardapio
            .Include(i => i.Ingredientes)
            .AsQueryable();

        if (periodo.HasValue)
            query = query.Where(i => i.Periodo == periodo.Value);

        var itens = await query.ToListAsync();

        return itens.Select(item =>
        {
            var sugestao = sugestoesHoje.FirstOrDefault(s =>
                s.ItemCardapioId == item.Id &&
                s.Periodo == item.Periodo);

            decimal? precoComDesconto = null;

            if (sugestao != null)
            {
                precoComDesconto = sugestao.AplicarDesconto(item.PrecoBase);
            }

            return new ItemCardapioResponseDto(
                item.Id,
                item.Nome,
                item.Descricao,
                item.PrecoBase,
                item.Periodo,
                sugestao != null,
                precoComDesconto,
                item.Ingredientes.Select(ing => ing.Nome).ToList()
            );
        }).ToList();
    }

    public async Task<ItemCardapio> CriarItemAsync(ItemCardapioDto dto)
    {
        var contePorPeriodo = await _context.ItensCardapio
            .CountAsync(i => i.Periodo == dto.Periodo);

        if (contePorPeriodo >= 20)
            throw new InvalidOperationException($"O cardápio já possui 20 itens para o período {dto.Periodo}.");

        var ingredientes = await _context.Ingredientes
            .Where(i => dto.IngredienteIds.Contains(i.Id))
            .ToListAsync();

        var item = new ItemCardapio
        {
            Nome = dto.Nome,
            Descricao = dto.Descricao,
            PrecoBase = dto.PrecoBase,
            Periodo = dto.Periodo,
            Ingredientes = ingredientes
        };

        _context.ItensCardapio.Add(item);
        await _context.SaveChangesAsync();
        return item;
    }
}


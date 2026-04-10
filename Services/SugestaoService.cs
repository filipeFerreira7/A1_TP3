namespace A1_order_system.Services;
using A1_order_system.Data;
using A1_order_system.Dtos;
using A1_order_system.Entities;
using Microsoft.EntityFrameworkCore;

public class SugestaoService
{
    private readonly RestauranteDbContext _context;

    public SugestaoService(RestauranteDbContext context) => _context = context;

    public async Task<SugestaoResponseDto> DefinirSugestaoAsync(SugestaoDto dto, long usuarioId)
    {
        var hoje = DateTime.UtcNow.Date;

        // Regra: apenas 1 sugestão por período por dia
        var jaExiste = await _context.SugestoesChefe
            .AnyAsync(s => s.Data.Date == hoje && s.Periodo == dto.Periodo);

        if (jaExiste)
            throw new InvalidOperationException(
                $"Já existe uma Sugestão do Chefe definida para {dto.Periodo} hoje.");

        var item = await _context.ItensCardapio.FindAsync(dto.ItemCardapioId)
            ?? throw new KeyNotFoundException("Item do cardápio não encontrado.");

        if (item.Periodo != dto.Periodo)
            throw new InvalidOperationException(
                $"O item '{item.Nome}' não pertence ao período {dto.Periodo}.");

        var sugestao = new SugestaoChefe
        {
            Data = hoje,
            Periodo = dto.Periodo,
            ItemCardapioId = dto.ItemCardapioId,
            UsuarioId = usuarioId
        };

        _context.SugestoesChefe.Add(sugestao);
        await _context.SaveChangesAsync();

        return new SugestaoResponseDto(sugestao.Id, sugestao.Data, sugestao.Periodo, item.Nome, sugestao.Desconto);
    }

    public async Task<List<SugestaoResponseDto>> SugestoesHojeAsync()
    {
        var hoje = DateTime.UtcNow.Date;

        return await _context.SugestoesChefe
            .Include(s => s.ItemCardapio)
            .Where(s => s.Data.Date == hoje)
            .Select(s => new SugestaoResponseDto(
                s.Id, s.Data, s.Periodo, s.ItemCardapio.Nome, s.Desconto))
            .ToListAsync();
    }
}
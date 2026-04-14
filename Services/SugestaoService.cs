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
            throw new InvalidOperationException($"Já existe uma Sugestão do Chefe definida para {dto.Periodo} hoje.");

        var item = await _context.ItensCardapio.FindAsync(dto.ItemCardapioId)
            ?? throw new KeyNotFoundException("Item do cardápio não encontrado.");

        if (item.Periodo != dto.Periodo)
            throw new InvalidOperationException($"O item '{item.Nome}' não pertence ao período {dto.Periodo}.");

        var sugestao = new SugestaoChefe
        {
            Data = hoje,
            Periodo = dto.Periodo,
            ItemCardapioId = dto.ItemCardapioId,
            UsuarioId = usuarioId,
            Desconto = 0.20m
        };

        _context.SugestoesChefe.Add(sugestao);
        await _context.SaveChangesAsync();

        // Retorna com dados completos
        return new SugestaoResponseDto
        {
            Id = sugestao.Id,
            Data = sugestao.Data,
            Periodo = sugestao.Periodo,
            NomeItem = item.Nome,
            PrecoBase = item.PrecoBase,
            PrecoComDesconto = item.PrecoBase * 0.8m,
            Desconto = sugestao.Desconto
        };
    }

    public async Task<List<SugestaoResponseDto>> SugestoesHojeAsync()
    {
        var hoje = DateTime.UtcNow.Date;

        return await _context.SugestoesChefe
            .Include(s => s.ItemCardapio)
            .Where(s => s.Data.Date == hoje)
            .Select(s => new SugestaoResponseDto
            {
                Id = s.Id,
                Data = s.Data,
                Periodo = s.Periodo,
                NomeItem = s.ItemCardapio.Nome,
                PrecoBase = s.ItemCardapio.PrecoBase,
                PrecoComDesconto = s.ItemCardapio.PrecoBase * 0.8m,   // 20% de desconto
                Desconto = s.Desconto
            })
            .ToListAsync();
    }
}
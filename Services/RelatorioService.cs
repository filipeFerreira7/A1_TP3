namespace A1_order_system.Services;
using A1_order_system.Data;
using A1_order_system.Dtos;
using A1_order_system.Entities;
using Microsoft.EntityFrameworkCore;


public class RelatorioService
{
    private readonly RestauranteDbContext _context;

    public RelatorioService(RestauranteDbContext context) => _context = context;

    public async Task<List<RelatorioFaturamentoDto>> FaturamentoPorTipoAsync(DateTime inicio, DateTime fim)
    {
        var pedidos = await _context.Pedidos
            .Include(p => p.Atendimento)
            .Where(p => p.Data.Date >= inicio.Date && p.Data.Date <= fim.Date)
            .ToListAsync();

        return pedidos
            .GroupBy(p => p.Atendimento switch
            {
                AtendimentoPresencial => "Presencial",
                AtendimentoDeliveryProprio => "DeliveryProprio",
                AtendimentoDeliveryApp => "DeliveryApp",
                _ => "Desconhecido"
            })
            .Select(g => new RelatorioFaturamentoDto(
                g.Key,
                g.Sum(p => p.ValorTotal),
                g.Count()))
            .OrderByDescending(r => r.TotalFaturado)
            .ToList();
    }

    public async Task<List<RelatorioItensMaisVendidosDto>> ItensMaisVendidosAsync(DateTime inicio, DateTime fim)
    {
        // Busca todos os pedido-itens no período
        var pedidoItens = await _context.PedidoItens
            .Include(pi => pi.ItemCardapio)
            .Include(pi => pi.Pedido)
            .Where(pi => pi.Pedido.Data.Date >= inicio.Date && pi.Pedido.Data.Date <= fim.Date)
            .ToListAsync();

        // Sugestões do período para marcar quantas vezes um item foi vendido como sugestão
        var sugestoes = await _context.SugestoesChefe
            .Where(s => s.Data.Date >= inicio.Date && s.Data.Date <= fim.Date)
            .ToListAsync();

        var sugestaoIds = sugestoes.Select(s => s.ItemCardapioId).ToHashSet();

        return pedidoItens
            .GroupBy(pi => pi.ItemCardapioId)
            .Select(g =>
            {
                var item = g.First().ItemCardapio;
                int comoSugestao = g.Count(pi => sugestaoIds.Contains(pi.ItemCardapioId));

                return new RelatorioItensMaisVendidosDto(
                    item.Id,
                    item.Nome,
                    item.Periodo,
                    g.Sum(pi => pi.Quantidade),
                    comoSugestao);
            })
            .OrderByDescending(r => r.QuantidadeVendida)
            .ToList();
    }
}
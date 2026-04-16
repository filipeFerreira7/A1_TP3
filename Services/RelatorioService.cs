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
        var dataInicio = inicio.Date;
        var dataFim = fim.Date.AddDays(1).AddTicks(-1);

        var pedidos = await _context.Pedidos
            .Include(p => p.Atendimento)
            .Include(p => p.Itens)
                .ThenInclude(pi => pi.ItemCardapio)
            .Where(p => p.Data >= dataInicio && p.Data <= dataFim)
            .ToListAsync();

        if (!pedidos.Any())
            return new List<RelatorioFaturamentoDto>();

        var sugestoes = await _context.SugestoesChefe
            .Where(s => s.Data >= dataInicio && s.Data <= dataFim)
            .ToListAsync();

        return pedidos
            .Select(p =>
            {
                var tipoAtendimento = p.Atendimento switch
                {
                    AtendimentoPresencial => "Presencial",
                    AtendimentoDeliveryProprio => "DeliveryProprio",
                    AtendimentoDeliveryApp => "DeliveryApp",
                    _ => "Desconhecido"
                };

                var subtotalItens = CalcularSubtotalItens(p, sugestoes);
                var totalTaxas = Math.Max(0m, p.ValorTotal - subtotalItens);
                var receitaLiquida = p.ValorTotal - totalTaxas;

                return new
                {
                    TipoAtendimento = tipoAtendimento,
                    TotalFaturado = p.ValorTotal,
                    TotalTaxas = totalTaxas,
                    ReceitaLiquida = receitaLiquida
                };
            })
            .GroupBy(p => p.TipoAtendimento)
            .Select(g => new RelatorioFaturamentoDto(
                g.Key,
                g.Sum(p => p.TotalFaturado),
                g.Sum(p => p.TotalTaxas),
                g.Sum(p => p.ReceitaLiquida),
                g.Count()
            ))
            .OrderByDescending(r => r.TotalFaturado)
            .ToList();
    }

    private static decimal CalcularSubtotalItens(Pedido pedido, List<SugestaoChefe> sugestoes)
    {
        decimal subtotal = 0m;

        foreach (var item in pedido.Itens)
        {
            var precoBase = item.ItemCardapio.PrecoBase;
            var sugestao = sugestoes.FirstOrDefault(s =>
                s.ItemCardapioId == item.ItemCardapioId &&
                s.Periodo == pedido.Periodo &&
                s.Data.Date == pedido.Data.Date);

            var precoFinal = sugestao is null
                ? precoBase
                : sugestao.AplicarDesconto(precoBase);

            subtotal += precoFinal * item.Quantidade;
        }

        return subtotal;
    }

    public async Task<List<RelatorioItensMaisVendidosDto>> ItensMaisVendidosAsync(DateTime inicio, DateTime fim)
    {
        var pedidoItens = await _context.PedidoItens
            .Include(pi => pi.ItemCardapio)
            .Include(pi => pi.Pedido)
            .Where(pi => pi.Pedido.Data.Date >= inicio.Date && pi.Pedido.Data.Date <= fim.Date)
            .ToListAsync();

        var sugestoes = await _context.SugestoesChefe
            .Where(s => s.Data.Date >= inicio.Date && s.Data.Date <= fim.Date)
            .ToListAsync();

        return pedidoItens
            .GroupBy(pi => pi.ItemCardapioId)
            .Select(g =>
            {
                var item = g.First().ItemCardapio;
                int comoSugestao = g.Sum(pi =>
                {
                    var foiSugestaoNoDia = sugestoes.Any(s =>
                        s.ItemCardapioId == pi.ItemCardapioId &&
                        s.Periodo == pi.ItemCardapio.Periodo &&
                        s.Data.Date == pi.Pedido.Data.Date);

                    return foiSugestaoNoDia ? pi.Quantidade : 0;
                });

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

namespace A1_order_system.Services
{
    using A1_order_system.Data;
    using A1_order_system.Dtos;
    using A1_order_system.Entities;
    using Microsoft.EntityFrameworkCore;

    public class PedidoService
    {
        private readonly RestauranteDbContext _context;

        public PedidoService(RestauranteDbContext context) => _context = context;

        public async Task<PedidoResponseDto> CriarPedidoAsync(PedidoDto dto, long usuarioId)
        {
            if (dto.TipoAtendimento != "Presencial" && dto.EnderecoId == null)
                throw new InvalidOperationException("Endereco de entrega e obrigatorio para pedidos de delivery.");

            var idsItens = dto.Itens.Select(i => i.ItemCardapioId).ToList();
            var itensCardapio = await _context.ItensCardapio
                .Where(i => idsItens.Contains(i.Id))
                .ToListAsync();

            if (itensCardapio.Count != idsItens.Count)
                throw new KeyNotFoundException("Um ou mais itens do cardapio nao foram encontrados.");

            var periodoNome = dto.Periodo == Periodo.Almoco ? "Almoco" : "Jantar";
            var itensWrongPeriod = itensCardapio.Where(i => i.Periodo != dto.Periodo).ToList();
            if (itensWrongPeriod.Any())
            {
                var periodoItem = itensWrongPeriod[0].Periodo == Periodo.Almoco ? "Almoco" : "Jantar";
                throw new InvalidOperationException(
                    $"O item '{string.Join(", ", itensWrongPeriod.Select(i => i.Nome))}' pertence ao periodo de {periodoItem} e nao pode ser adicionado a um pedido de {periodoNome}.");
            }

            var taxaFixa = dto.TaxaFixa ?? 8.00m;

            Atendimento atendimento = dto.TipoAtendimento switch
            {
                "Presencial" => new AtendimentoPresencial(),
                "DeliveryProprio" => new AtendimentoDeliveryProprio { TaxaFixa = taxaFixa },
                "DeliveryApp" => new AtendimentoDeliveryApp { NomeApp = dto.NomeApp ?? "iFood" },
                _ => throw new InvalidOperationException($"Tipo de atendimento invalido: {dto.TipoAtendimento}.")
            };

            _context.Atendimentos.Add(atendimento);
            await _context.SaveChangesAsync();

            var pedido = new Pedido
            {
                Data = DateTime.UtcNow,
                Periodo = dto.Periodo,
                UsuarioId = usuarioId,
                AtendimentoId = atendimento.Id,
                EnderecoId = dto.EnderecoId
            };

            foreach (var dtoItem in dto.Itens)
            {
                pedido.Itens.Add(new PedidoItem
                {
                    ItemCardapioId = dtoItem.ItemCardapioId,
                    Quantidade = dtoItem.Quantidade
                });
            }

            _context.Pedidos.Add(pedido);
            await _context.SaveChangesAsync();

            await _context.Entry(pedido).Collection(p => p.Itens).Query()
                .Include(pi => pi.ItemCardapio).LoadAsync();
            await _context.Entry(pedido).Reference(p => p.Atendimento).LoadAsync();

            var hoje = DateTime.UtcNow.Date;
            var sugestaoAlmoco = await _context.SugestoesChefe
                .FirstOrDefaultAsync(s => s.Data.Date == hoje && s.Periodo == Periodo.Almoco);
            var sugestaoJantar = await _context.SugestoesChefe
                .FirstOrDefaultAsync(s => s.Data.Date == hoje && s.Periodo == Periodo.Jantar);

            pedido.CalcularValorTotal(sugestaoAlmoco, sugestaoJantar);
            await _context.SaveChangesAsync();

            return MapearResponse(pedido, sugestaoAlmoco, sugestaoJantar);
        }

        public async Task<List<PedidoResponseDto>> ListarPedidosUsuarioAsync(long usuarioId)
        {
            var pedidos = await _context.Pedidos
                .Include(p => p.Atendimento)
                .Include(p => p.Usuario)
                .Include(p => p.Endereco)
                .Include(p => p.Itens).ThenInclude(pi => pi.ItemCardapio)
                .Where(p => p.UsuarioId == usuarioId)
                .OrderByDescending(p => p.Data)
                .ToListAsync();

            var hoje = DateTime.UtcNow.Date;
            var sugestaoAlmoco = await _context.SugestoesChefe
                .FirstOrDefaultAsync(s => s.Data.Date == hoje && s.Periodo == Periodo.Almoco);
            var sugestaoJantar = await _context.SugestoesChefe
                .FirstOrDefaultAsync(s => s.Data.Date == hoje && s.Periodo == Periodo.Jantar);

            return pedidos.Select(p => MapearResponse(p, sugestaoAlmoco, sugestaoJantar)).ToList();
        }

        public async Task<List<PedidoResponseDto>> ListarTodosPedidosAsync()
        {
            var pedidos = await _context.Pedidos
                .Include(p => p.Atendimento)
                .Include(p => p.Usuario)
                .Include(p => p.Endereco)
                .Include(p => p.Itens).ThenInclude(pi => pi.ItemCardapio)
                .OrderByDescending(p => p.Data)
                .ToListAsync();

            var hoje = DateTime.UtcNow.Date;
            var sugestaoAlmoco = await _context.SugestoesChefe
                .FirstOrDefaultAsync(s => s.Data.Date == hoje && s.Periodo == Periodo.Almoco);
            var sugestaoJantar = await _context.SugestoesChefe
                .FirstOrDefaultAsync(s => s.Data.Date == hoje && s.Periodo == Periodo.Jantar);

            return pedidos.Select(p => MapearResponse(p, sugestaoAlmoco, sugestaoJantar)).ToList();
        }

        public async Task<List<PedidoResponseDto>> ListarPedidosPorUsuarioAsync(long usuarioId)
        {
            var pedidos = await _context.Pedidos
                .Include(p => p.Atendimento)
                .Include(p => p.Usuario)
                .Include(p => p.Endereco)
                .Include(p => p.Itens).ThenInclude(pi => pi.ItemCardapio)
                .Where(p => p.UsuarioId == usuarioId)
                .OrderByDescending(p => p.Data)
                .ToListAsync();

            var hoje = DateTime.UtcNow.Date;
            var sugestaoAlmoco = await _context.SugestoesChefe
                .FirstOrDefaultAsync(s => s.Data.Date == hoje && s.Periodo == Periodo.Almoco);
            var sugestaoJantar = await _context.SugestoesChefe
                .FirstOrDefaultAsync(s => s.Data.Date == hoje && s.Periodo == Periodo.Jantar);

            return pedidos.Select(p => MapearResponse(p, sugestaoAlmoco, sugestaoJantar)).ToList();
        }

        private static PedidoResponseDto MapearResponse(
            Pedido pedido,
            SugestaoChefe? sugestaoAlmoco,
            SugestaoChefe? sugestaoJantar)
        {
            string tipoAtendimento = pedido.Atendimento switch
            {
                AtendimentoPresencial => "Presencial",
                AtendimentoDeliveryProprio => "DeliveryProprio",
                AtendimentoDeliveryApp => "DeliveryApp",
                _ => "Desconhecido"
            };

            var itensResponse = pedido.Itens.Select(pi =>
            {
                bool temDesconto =
                    (sugestaoAlmoco != null && sugestaoAlmoco.ItemCardapioId == pi.ItemCardapioId
                        && pedido.Periodo == Periodo.Almoco
                        && sugestaoAlmoco.Data.Date == pedido.Data.Date) ||
                    (sugestaoJantar != null && sugestaoJantar.ItemCardapioId == pi.ItemCardapioId
                        && pedido.Periodo == Periodo.Jantar
                        && sugestaoJantar.Data.Date == pedido.Data.Date);

                decimal preco = temDesconto
                    ? (pedido.Periodo == Periodo.Almoco ? sugestaoAlmoco! : sugestaoJantar!)
                        .AplicarDesconto(pi.ItemCardapio.PrecoBase)
                    : pi.ItemCardapio.PrecoBase;

                return new PedidoItemResponseDto(
                    pi.ItemCardapioId, pi.ItemCardapio.Nome,
                    pi.Quantidade, preco, temDesconto);
            }).ToList();

            return new PedidoResponseDto(
                pedido.Id, pedido.Data, pedido.Periodo,
                tipoAtendimento, pedido.ValorTotal, itensResponse,
                pedido.Usuario?.Nome,
                pedido.UsuarioId,
                pedido.Endereco != null
                    ? new EnderecoResponseDto(
                        pedido.Endereco.Id,
                        pedido.Endereco.Logradouro,
                        pedido.Endereco.Cidade,
                        pedido.Endereco.Estado)
                    : null);
        }
    }
}

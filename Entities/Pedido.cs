namespace A1_order_system.Entities
{
    public class Pedido
    {
        public long Id { get; set; }
        public DateTime Data { get; set; } = DateTime.UtcNow;
        public decimal ValorTotal { get; private set; }
        public Periodo Periodo { get; set; }

        public long UsuarioId { get; set; }
        public Usuario Usuario { get; set; } = null!;

        public long AtendimentoId { get; set; }
        public Atendimento Atendimento { get; set; } = null!;

        public long? EnderecoId { get; set; }
        public Endereco? Endereco { get; set; }

        public ICollection<PedidoItem> Itens { get; set; } = new List<PedidoItem>();

        public void CalcularValorTotal(SugestaoChefe? sugestaoAlmoco, SugestaoChefe? sugestaoJantar)
        {
            decimal subtotal = 0m;

            foreach (var item in Itens)
            {
                var preco = item.ItemCardapio.PrecoBase;

                bool ehSugestaoAlmoco = sugestaoAlmoco != null
                    && sugestaoAlmoco.ItemCardapioId == item.ItemCardapioId
                    && sugestaoAlmoco.Data.Date == Data.Date
                    && Periodo == Periodo.Almoco;

                bool ehSugestaoJantar = sugestaoJantar != null
                    && sugestaoJantar.ItemCardapioId == item.ItemCardapioId
                    && sugestaoJantar.Data.Date == Data.Date
                    && Periodo == Periodo.Jantar;

                if (ehSugestaoAlmoco)
                    preco = sugestaoAlmoco!.AplicarDesconto(preco);
                else if (ehSugestaoJantar)
                    preco = sugestaoJantar!.AplicarDesconto(preco);

                subtotal += preco * item.Quantidade;
            }

            decimal taxa = Atendimento?.CalcularTaxa(subtotal, Data) ?? 0m;
            ValorTotal = subtotal + taxa;
        }

        public void SetValorTotal(decimal valor) => ValorTotal = valor;
    }

    public class PedidoItem
    {
        public long Id { get; set; }
        public int Quantidade { get; set; } = 1;

        public long PedidoId { get; set; }
        public Pedido Pedido { get; set; } = null!;

        public long ItemCardapioId { get; set; }
        public ItemCardapio ItemCardapio { get; set; } = null!;
    }
}


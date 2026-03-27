namespace A1_order_system.Entities
{
    public class SugestaoChefe
    {
        public long Id { get; set; }
        public DateTime Data { get; set; }

        public Periodo Periodo { get; set; }

        public decimal Desconto { get; set; } = 0.20m;

        public long ItemCardapioId { get; set; }

        public ItemCardapio ItemCardapio { get; set; } = null!;

        public long UsuarioId { get; set; }

        public Usuario Usuario { get; set; } = null!;


        public decimal AplicarDesconto(decimal precoBase) => precoBase * (1 - Desconto);


    }
}

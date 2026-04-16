namespace A1_order_system.Entities
{
    public class ItemCardapio
    {
        public long Id {  get; set; }
        public string Nome {get; set; } = string.Empty;
        public string Descricao {get; set; } = string.Empty;
        public decimal PrecoBase {  get; set; }
        public Periodo Periodo {get; set; }

        public ICollection<Ingrediente> Ingredientes { get; set; } = new List<Ingrediente>();
        public ICollection<SugestaoChefe> SugestaoChefes { get; set; } = new List<SugestaoChefe>();

        public ICollection<PedidoItem> PedidoItens { get; set; } = new List <PedidoItem>();


    }
}

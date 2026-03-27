namespace A1_order_system.Entities
{
    public class Ingrediente
    {
        public long Id { get; set; }
        public string Nome { get; set; } = string.Empty;

        public ICollection<ItemCardapio> ItensCardapio { get ; set; } = new List<ItemCardapio>();

    }
}
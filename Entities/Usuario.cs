namespace A1_order_system.Entities
{
    public class Usuario
    {
        public long Id { get; set; }
        public string Nome { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Senha { get; set; } = string.Empty;

        public ICollection<Endereco> Enderecos { get; set; } = new List<Endereco>();
        public ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
        public ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
    }
}

namespace A1_order_system.Entities
{
    public class Mesa
    {
        public long Id { get; set; }
        public int Numero { get; set; }

        public ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
    }
}
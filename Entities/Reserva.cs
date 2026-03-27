namespace A1_order_system.Entities
{
    public class Reserva
    {
      
     

        public int Id { get; set; }
        public DateTime DataHora { get; set; }

        public string NomeDoCliente { get; set; } = string.Empty;

        public long MesaId { get; set; }
        public Mesa Mesa { get; set; } = null!;

        public long UsuarioId { get; set; }
        public Usuario Usuario { get; set; } = null!;


    }
}


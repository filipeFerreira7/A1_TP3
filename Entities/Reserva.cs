namespace A1_order_system.Entities
{
    public class Reserva
    {

        public static readonly TimeSpan HorarioInicio = new(19, 0, 0);
        public static readonly TimeSpan HorarioFim = new(22, 0, 0);

        public int Id { get; set; }
        public DateTime DataHora { get; set; }

        public string NomeDoCliente { get; set; } = string.Empty;

        public long MesaId { get; set; }
        public Mesa Mesa { get; set; } = null!;

        public long UsuarioId { get; set; }
        public Usuario Usuario { get; set; } = null!;

        public bool HorarioValido()
        {
            var hora = DataHora.TimeOfDay;
            return hora >= HorarioInicio && hora <= HorarioFim;
        }


    }
}


namespace A1_order_system.Entities
{

    // Classe base abstrata conforme diagrama
    public abstract class Atendimento
    {
        public long Id { get; set; }

        // Cada subclasse calcula sua própria taxa
        public abstract decimal CalcularTaxa(decimal valorPedido, DateTime horario);

        public Pedido? Pedido { get; set; }
    }

    public class AtendimentoPresencial : Atendimento
    {
        public override decimal CalcularTaxa(decimal valorPedido, DateTime horario) => 0m;
    }

    public class AtendimentoDeliveryProprio : Atendimento
    {
        public decimal TaxaFixa { get; set; }

        public override decimal CalcularTaxa(decimal valorPedido, DateTime horario) => TaxaFixa;
    }

    public class AtendimentoDeliveryApp : Atendimento
    {
        public string NomeApp { get; set; } = string.Empty;

        // Regra de negócio: 4% diurno, 6% noturno (a partir das 18h)
        public override decimal CalcularTaxa(decimal valorPedido, DateTime horario)
        {
            bool noturno = horario.Hour >= 18;
            decimal percentual = noturno ? 0.06m : 0.04m;
            return valorPedido * percentual;
        }
    }
}

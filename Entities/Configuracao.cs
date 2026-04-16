namespace A1_order_system.Entities
{
    public class Configuracao
    {
        public long Id { get; set; }
        public string Chave { get; set; } = string.Empty;
        public string Valor { get; set; } = string.Empty;

        public static class Keys
        {
            public const string TaxaDeliveryProprio = "TaxaDeliveryProprio";
            public const decimal TaxaDeliveryProprioDefault = 8.00m;
        }
    }
}

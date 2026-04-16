namespace A1_order_system.Dtos
{
    public record SugestaoDto
       (
       long ItemCardapioId,
       Periodo Periodo
       )
    {
    }

    public record SugestaoResponseDto
    {
        public long Id { get; init; }
        public DateTime Data { get; init; }
        public Periodo Periodo { get; init; }
        public string NomeItem { get; init; } = string.Empty;
        public decimal PrecoBase { get; init; }
        public decimal PrecoComDesconto { get; init; }
        public decimal Desconto { get; init; }

        public SugestaoResponseDto(long id, DateTime data, Periodo periodo, string nomeItem, decimal desconto)
        {
            Id = id;
            Data = data;
            Periodo = periodo;
            NomeItem = nomeItem;
            Desconto = desconto;
            PrecoBase = 0;
            PrecoComDesconto = 0;
        }

        public SugestaoResponseDto() { }
    }
}


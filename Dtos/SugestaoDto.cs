namespace A1_order_system.Dtos
{
    public record SugestaoDto
       (
       long ItemCardapioId,
       Periodo Periodo
       )
    {
    }

    // DTO para retornar sugestões do chefe (usado em SugestoesHojeAsync)
    public record SugestaoResponseDto
    {
        public long Id { get; init; }
        public DateTime Data { get; init; }
        public Periodo Periodo { get; init; }
        public string NomeItem { get; init; } = string.Empty;
        public decimal PrecoBase { get; init; }
        public decimal PrecoComDesconto { get; init; }
        public decimal Desconto { get; init; }

        // Construtor para manter compatibilidade com DefinirSugestaoAsync
        public SugestaoResponseDto(long id, DateTime data, Periodo periodo, string nomeItem, decimal desconto)
        {
            Id = id;
            Data = data;
            Periodo = periodo;
            NomeItem = nomeItem;
            Desconto = desconto;
            // Preenche os preços (mesmo que não venha do banco ainda)
            PrecoBase = 0; // será sobrescrito no Select se necessário
            PrecoComDesconto = 0;
        }

        // Construtor vazio para o Select do EF Core
        public SugestaoResponseDto() { }
    }
}
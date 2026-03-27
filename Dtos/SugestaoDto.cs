namespace A1_order_system.Dtos
{
    public record SugestaoDto
        (
        long ItemCardapioId,
        Periodo Periodo
        )
    {
    }
}

namespace A1_order_system.Dtos
{
    public record SugestaoResponseDto
        (
        long Id,
        DateTime Data,
        Periodo Periodo,
        string ItemNome, 
        decimal Desconto
        )

    { }
}

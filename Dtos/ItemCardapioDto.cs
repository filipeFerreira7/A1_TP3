namespace A1_order_system.Dtos
{
    public record ItemCardapioDto
        (
        string Nome,
        string Descricao,
        decimal PrecoBase,
        Periodo Periodo,
        List<long> IngredienteIds
        )
    {
    }
}
namespace A1_order_system.Dtos
{
    public record ItemCardapioResponseDto
        (
        long Id,
        string Nome,
        string Descricao,
        decimal PrecoBase,
        Periodo Periodo,
        bool IsSugestaoChefe,
        decimal? PrecoComDesconto,
        List<String> Ingredientes
        )
    {

    }
}

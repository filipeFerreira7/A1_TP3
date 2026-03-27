using A1_order_system;

namespace A1_order_system.Dtos
{
    public record PedidoDto
        (
        Periodo Periodo,
        string TipoAtendimento,        // "Presencial" | "DeliveryProprio" | "DeliveryApp"
        List<PedidoItemDto> Itens,
        decimal? TaxaFixa,             // para DeliveryProprio
        string? NomeApp,               // para DeliveryApp
        long? EnderecoId
        );
    
}

namespace A1_order_system.Dtos
{
    public record PedidoItemDto
        (
        long ItemCardapioId,
        int Quantidade

        );
}
namespace A1_order_system.Dtos {
    public record PedidoResponseDto(
        long Id,
        DateTime Data,
        Periodo Periodo,
        string TipoAtendimento,
        decimal ValorTotal,
        List<PedidoItemResponseDto> Itens);
}
namespace A1_order_system.Dtos
{
    public record PedidoItemResponseDto(
        long ItemCardapioId,
        string Nome,
        int Quantidade,
        decimal PrecoUnitario,
        bool TemDesconto);
}
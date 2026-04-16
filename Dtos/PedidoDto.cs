using A1_order_system;

namespace A1_order_system.Dtos
{
    public record PedidoDto
        (
        Periodo Periodo,
        string TipoAtendimento,
        List<PedidoItemDto> Itens,
        decimal? TaxaFixa,
        string? NomeApp,
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
        List<PedidoItemResponseDto> Itens,
        string? NomeUsuario = null,
        long? UsuarioId = null,
        EnderecoResponseDto? Endereco = null);
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


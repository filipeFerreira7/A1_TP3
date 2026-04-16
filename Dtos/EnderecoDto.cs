namespace A1_order_system.Dtos
{
    public record EnderecoDto 
        (
        string Logradouro,
        string Cidade,
        string Estado
        )
    {
    }
}
namespace A1_order_system.Dtos
{
    public record EnderecoResponseDto
        (
        long Id,
        string Logradouro,
        string Cidade,
        string Estado,
        string? ClienteNome = null,
        string? ClienteEmail = null
        )
    { }
}

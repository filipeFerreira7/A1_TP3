namespace A1_order_system.Dtos
{
    public record AuthDto
        (
        string Nome,
        string Email,
        string Senha
        );
    public record LoginDto(string Email, string Senha);
    public record TokenDto(string Token, string Nome, string Email);
}

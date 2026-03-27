namespace A1_order_system.Dtos
{
    public record ReservaDto
        (
        DateTime Horario,
        string NomeDoCliente,
        long MesaId
        );

    public record ReservaResponseDto(
        long Id,
        DateTime Data,
        DateTime Horario,
        string NomeDoCliente,
        int NumeroMesa,
        string CodigoConfirmacao);

}

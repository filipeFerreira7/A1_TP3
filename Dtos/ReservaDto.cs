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
        string NomeDoCliente,
        int NumeroMesa
        );

}

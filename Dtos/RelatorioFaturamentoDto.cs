namespace A1_order_system.Dtos
{
    public record RelatorioFaturamentoDto
    (
        string TipoAtendimento,
        decimal TotalFaturado,
        int TotalPedidos
    );

    public record RelatorioItensMaisVendidosDto(
    long ItemId,
    string Nome,
    Periodo Periodo,
    int QuantidadeVendida,
    int QuantidadeComoSugestao);
}

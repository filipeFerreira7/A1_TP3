namespace A1_order_system.Dtos
{
    public record RelatorioFaturamentoDto
    (
        string TipoAtendimento,
        decimal TotalFaturado,
        decimal TotalTaxas,
        decimal ReceitaLiquida,
        int TotalPedidos
    );

    public record RelatorioItensMaisVendidosDto(
    long ItemId,
    string Nome,
    Periodo Periodo,
    int QuantidadeVendida,
    int QuantidadeComoSugestao);
}

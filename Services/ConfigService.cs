namespace A1_order_system.Services
{
    using A1_order_system.Data;
    using A1_order_system.Entities;
    using Microsoft.EntityFrameworkCore;

    public class ConfigService
    {
        private readonly RestauranteDbContext _context;

        public ConfigService(RestauranteDbContext context) => _context = context;

        public async Task<decimal> GetTaxaDeliveryProprioAsync()
        {
            var config = await _context.Configuracoes
                .FirstOrDefaultAsync(c => c.Chave == Configuracao.Keys.TaxaDeliveryProprio);

            if (config == null)
            {
                config = new Configuracao
                {
                    Chave = Configuracao.Keys.TaxaDeliveryProprio,
                    Valor = Configuracao.Keys.TaxaDeliveryProprioDefault.ToString("F2")
                };
                _context.Configuracoes.Add(config);
                await _context.SaveChangesAsync();
            }

            return decimal.TryParse(config.Valor, out var taxa) ? taxa : Configuracao.Keys.TaxaDeliveryProprioDefault;
        }

        public async Task SetTaxaDeliveryProprioAsync(decimal taxa)
        {
            var config = await _context.Configuracoes
                .FirstOrDefaultAsync(c => c.Chave == Configuracao.Keys.TaxaDeliveryProprio);

            if (config == null)
            {
                config = new Configuracao
                {
                    Chave = Configuracao.Keys.TaxaDeliveryProprio,
                    Valor = taxa.ToString("F2")
                };
                _context.Configuracoes.Add(config);
            }
            else
            {
                config.Valor = taxa.ToString("F2");
            }

            await _context.SaveChangesAsync();
        }
    }
}

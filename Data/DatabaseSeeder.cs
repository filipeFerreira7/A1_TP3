using A1_order_system.Data;
using A1_order_system.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace A1_order_system.Data
{
    public static class DatabaseSeeder
    {
        public static async Task SeedAsync(RestauranteDbContext db)
        {
            await db.Database.MigrateAsync();
            await GarantirColunaPerfilAsync(db);

            using var transaction = await db.Database.BeginTransactionAsync();

            try
            {
                await SeedUsuariosAsync(db);
                await SeedIngredientesAsync(db);
                await SeedMesasAsync(db);

                await db.SaveChangesAsync();

                await SeedItensCardapioAsync(db);

                await db.SaveChangesAsync();
                await transaction.CommitAsync();

                Console.WriteLine("âœ… Database seeding concluÃ­do com sucesso!");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"âŒ Erro no seeding: {ex.Message}");
                throw;
            }
        }

        private static async Task GarantirColunaPerfilAsync(RestauranteDbContext db)
        {
            const string ensureColumnSql = @"
IF COL_LENGTH('Usuarios', 'Perfil') IS NULL
BEGIN
    ALTER TABLE Usuarios
    ADD Perfil nvarchar(20) NOT NULL CONSTRAINT DF_Usuarios_Perfil DEFAULT 'Cliente';
END";

            const string promoteAdminSql = @"
IF COL_LENGTH('Usuarios', 'Perfil') IS NOT NULL
BEGIN
    UPDATE Usuarios
    SET Perfil = 'Admin'
    WHERE Email = 'admin@restaurante.com';
END";

            await db.Database.ExecuteSqlRawAsync(ensureColumnSql);
            await db.Database.ExecuteSqlRawAsync(promoteAdminSql);
        }

        private static async Task SeedUsuariosAsync(RestauranteDbContext db)
        {
            var admin = await db.Usuarios
                .FirstOrDefaultAsync(u => u.Email == "admin@restaurante.com");

            if (admin is null)
            {
                admin = new Usuario
                {
                    Nome = "Administrador",
                    Email = "admin@restaurante.com",
                    Senha = BCrypt.Net.BCrypt.HashPassword("admin123"),
                    Perfil = PerfisUsuario.Admin
                };

                db.Usuarios.Add(admin);
                Console.WriteLine("âœ“ UsuÃ¡rio Admin criado (admin@restaurante.com / admin123)");
                return;
            }

            admin.Nome = "Administrador";
            admin.Perfil = PerfisUsuario.Admin;
            Console.WriteLine("âœ“ UsuÃ¡rio Admin validado/atualizado.");
        }

        private static async Task SeedIngredientesAsync(RestauranteDbContext db)
        {
            if (await db.Ingredientes.AnyAsync()) return;

            var ingredientes = new List<Ingrediente>
            {
                new Ingrediente { Nome = "Arroz Branco" },
                new Ingrediente { Nome = "FeijÃ£o Carioca" },
                new Ingrediente { Nome = "Carne de Alcatra" },
                new Ingrediente { Nome = "Frango Desossado" },
                new Ingrediente { Nome = "Carne MoÃ­da" },
                new Ingrediente { Nome = "Peito de Peru" },
                new Ingrediente { Nome = "SalmÃ£o" },
                new Ingrediente { Nome = "Tomate" },
                new Ingrediente { Nome = "Cebola" },
                new Ingrediente { Nome = "Alho" },
                new Ingrediente { Nome = "PimentÃ£o" },
                new Ingrediente { Nome = "Batata" },
                new Ingrediente { Nome = "Queijo Mussarela" },
                new Ingrediente { Nome = "Queijo ParmesÃ£o" },
                new Ingrediente { Nome = "MacarrÃ£o Espaguete" },
                new Ingrediente { Nome = "MacarrÃ£o Penne" },
                new Ingrediente { Nome = "Molho de Tomate" },
                new Ingrediente { Nome = "Molho Branco" },
                new Ingrediente { Nome = "Ã“leo de Soja" },
                new Ingrediente { Nome = "Azeite" },
                new Ingrediente { Nome = "Ovo" },
                new Ingrediente { Nome = "Leite" },
                new Ingrediente { Nome = "Creme de Leite" }
            };

            db.Ingredientes.AddRange(ingredientes);
            Console.WriteLine($"âœ“ {ingredientes.Count} ingredientes adicionados.");
        }

        private static async Task SeedMesasAsync(RestauranteDbContext db)
        {
            if (await db.Mesas.AnyAsync()) return;

            var mesas = Enumerable.Range(1, 12)
                .Select(i => new Mesa { Numero = i })
                .ToList();

            db.Mesas.AddRange(mesas);
            Console.WriteLine($"âœ“ {mesas.Count} mesas criadas (1 a 12).");
        }

        private static async Task SeedItensCardapioAsync(RestauranteDbContext db)
        {
            if (await db.ItensCardapio.CountAsync() >= 40)
            {
                Console.WriteLine("âœ“ Itens do cardÃ¡pio jÃ¡ existem (40 ou mais). Pulando seed.");
                return;
            }

            Console.WriteLine("ðŸ”„ Iniciando seed de 40 itens do cardÃ¡pio...");

            var ing = await db.Ingredientes.ToDictionaryAsync(i => i.Nome, StringComparer.OrdinalIgnoreCase);

            var itens = new List<ItemCardapio>();

            itens.AddRange(new[]
            {
                new ItemCardapio { Nome = "Arroz com FeijÃ£o e Bife", Descricao = "Arroz branco, feijÃ£o carioca e bife grelhado", PrecoBase = 32.90m, Periodo = Periodo.Almoco, Ingredientes = [ing["Arroz Branco"], ing["FeijÃ£o Carioca"], ing["Carne de Alcatra"]] },
                new ItemCardapio { Nome = "Frango Grelhado com Arroz", Descricao = "FilÃ© de frango grelhado, arroz e salada", PrecoBase = 29.90m, Periodo = Periodo.Almoco, Ingredientes = [ing["Arroz Branco"], ing["Frango Desossado"]] },
                new ItemCardapio { Nome = "Strogonoff de Frango", Descricao = "Strogonoff cremoso de frango com arroz", PrecoBase = 34.90m, Periodo = Periodo.Almoco, Ingredientes = [ing["Frango Desossado"], ing["Creme de Leite"], ing["Arroz Branco"]] },
                new ItemCardapio { Nome = "Feijoada Completa", Descricao = "Feijoada tradicional com arroz, farofa e couve", PrecoBase = 38.90m, Periodo = Periodo.Almoco },
                new ItemCardapio { Nome = "Bife Ã  Parmegiana", Descricao = "Bife empanado com molho de tomate e queijo", PrecoBase = 36.90m, Periodo = Periodo.Almoco, Ingredientes = [ing["Carne de Alcatra"], ing["Queijo Mussarela"], ing["Molho de Tomate"]] },
                new ItemCardapio { Nome = "Lasanha de Carne", Descricao = "Lasanha de carne moÃ­da com queijo gratinado", PrecoBase = 37.90m, Periodo = Periodo.Almoco, Ingredientes = [ing["Carne MoÃ­da"], ing["Queijo Mussarela"], ing["Molho de Tomate"]] },
                new ItemCardapio { Nome = "Peito de Peru Grelhado", Descricao = "Peito de peru com arroz integral e legumes", PrecoBase = 31.90m, Periodo = Periodo.Almoco },
                new ItemCardapio { Nome = "Arroz de Carreteiro", Descricao = "Arroz com carne seca, linguiÃ§a e ovos", PrecoBase = 33.90m, Periodo = Periodo.Almoco, Ingredientes = [ing["Arroz Branco"], ing["Ovo"]] },
                new ItemCardapio { Nome = "BobÃ³ de CamarÃ£o", Descricao = "BobÃ³ de camarÃ£o com arroz branco", PrecoBase = 42.90m, Periodo = Periodo.Almoco },
                new ItemCardapio { Nome = "Moqueca de Peixe", Descricao = "Moqueca de peixe com arroz e pirÃ£o", PrecoBase = 41.90m, Periodo = Periodo.Almoco },
                new ItemCardapio { Nome = "Virado Ã  Paulista", Descricao = "Virado com arroz, feijÃ£o, ovo e linguiÃ§a", PrecoBase = 30.90m, Periodo = Periodo.Almoco, Ingredientes = [ing["Arroz Branco"], ing["FeijÃ£o Carioca"], ing["Ovo"]] },
                new ItemCardapio { Nome = "FilÃ© de Frango Ã  Milanesa", Descricao = "FilÃ© de frango empanado com arroz e batata", PrecoBase = 28.90m, Periodo = Periodo.Almoco, Ingredientes = [ing["Frango Desossado"], ing["Batata"]] },
                new ItemCardapio { Nome = "Risoto de Frango", Descricao = "Risoto cremoso de frango", PrecoBase = 35.90m, Periodo = Periodo.Almoco },
                new ItemCardapio { Nome = "MacarrÃ£o ao Molho Branco com Frango", Descricao = "MacarrÃ£o penne ao molho branco com frango", PrecoBase = 32.90m, Periodo = Periodo.Almoco, Ingredientes = [ing["MacarrÃ£o Penne"], ing["Molho Branco"], ing["Frango Desossado"]] },
                new ItemCardapio { Nome = "Carne de Panela com Batata", Descricao = "Carne cozida com batatas e molho", PrecoBase = 33.90m, Periodo = Periodo.Almoco, Ingredientes = [ing["Batata"]] },
                new ItemCardapio { Nome = "Omelete Completo", Descricao = "Omelete com queijo, tomate e cebola", PrecoBase = 24.90m, Periodo = Periodo.Almoco, Ingredientes = [ing["Ovo"], ing["Queijo Mussarela"], ing["Tomate"]] },
                new ItemCardapio { Nome = "Salada de Frango Grelhado", Descricao = "Salada completa com filÃ© de frango", PrecoBase = 27.90m, Periodo = Periodo.Almoco },
                new ItemCardapio { Nome = "Prato Executivo", Descricao = "Arroz, feijÃ£o, bife, ovo e batata frita", PrecoBase = 26.90m, Periodo = Periodo.Almoco },
                new ItemCardapio { Nome = "Estrogonofe de Carne", Descricao = "Estrogonofe de carne com arroz", PrecoBase = 36.90m, Periodo = Periodo.Almoco },
                new ItemCardapio { Nome = "Peixe Grelhado com Legumes", Descricao = "FilÃ© de peixe grelhado com legumes", PrecoBase = 34.90m, Periodo = Periodo.Almoco }
            });

            itens.AddRange(new[]
            {
                new ItemCardapio { Nome = "Espaguete Ã  Carbonara", Descricao = "Espaguete com molho carbonara e bacon", PrecoBase = 38.90m, Periodo = Periodo.Jantar, Ingredientes = [ing["MacarrÃ£o Espaguete"], ing["Ovo"], ing["Queijo ParmesÃ£o"]] },
                new ItemCardapio { Nome = "Lasanha Ã  Bolonhesa", Descricao = "Lasanha tradicional Ã  bolonhesa com queijo", PrecoBase = 39.90m, Periodo = Periodo.Jantar, Ingredientes = [ing["MacarrÃ£o Penne"], ing["Carne MoÃ­da"], ing["Queijo Mussarela"]] },
                new ItemCardapio { Nome = "FilÃ© Mignon ao Molho Madeira", Descricao = "FilÃ© mignon com molho madeira e batata sautÃ©", PrecoBase = 52.90m, Periodo = Periodo.Jantar, Ingredientes = [ing["Batata"]] },
                new ItemCardapio { Nome = "SalmÃ£o Grelhado", Descricao = "SalmÃ£o grelhado com molho de alcaparras", PrecoBase = 48.90m, Periodo = Periodo.Jantar, Ingredientes = [ing["SalmÃ£o"]] },
                new ItemCardapio { Nome = "Pizza Margherita", Descricao = "Pizza de mussarela, tomate e manjericÃ£o", PrecoBase = 42.90m, Periodo = Periodo.Jantar, Ingredientes = [ing["Queijo Mussarela"], ing["Tomate"]] },
                new ItemCardapio { Nome = "Risoto de Funghi", Descricao = "Risoto de cogumelos com parmesÃ£o", PrecoBase = 41.90m, Periodo = Periodo.Jantar },
                new ItemCardapio { Nome = "Penne ao Molho Pesto", Descricao = "Penne com molho pesto e tomate seco", PrecoBase = 36.90m, Periodo = Periodo.Jantar, Ingredientes = [ing["MacarrÃ£o Penne"], ing["Tomate"]] },
                new ItemCardapio { Nome = "Frango Ã  Parmegiana", Descricao = "FilÃ© de frango empanado com molho e queijo", PrecoBase = 37.90m, Periodo = Periodo.Jantar, Ingredientes = [ing["Frango Desossado"], ing["Queijo Mussarela"]] },
                new ItemCardapio { Nome = "Lombo Ã  CalifÃ³rnia", Descricao = "Lombo com abacaxi e molho agridoce", PrecoBase = 39.90m, Periodo = Periodo.Jantar },
                new ItemCardapio { Nome = "CamarÃ£o ao Thermidor", Descricao = "CamarÃ£o gratinado com molho thermidor", PrecoBase = 49.90m, Periodo = Periodo.Jantar },
                new ItemCardapio { Nome = "Bife Ancho com Batata", Descricao = "Bife ancho grelhado com batata rÃºstica", PrecoBase = 45.90m, Periodo = Periodo.Jantar, Ingredientes = [ing["Batata"]] },
                new ItemCardapio { Nome = "MacarrÃ£o com CamarÃ£o", Descricao = "Espaguete com camarÃ£o e alho", PrecoBase = 43.90m, Periodo = Periodo.Jantar, Ingredientes = [ing["MacarrÃ£o Espaguete"]] },
                new ItemCardapio { Nome = "Polenta com RagÃº de LinguiÃ§a", Descricao = "Polenta cremosa com ragÃº de linguiÃ§a", PrecoBase = 35.90m, Periodo = Periodo.Jantar },
                new ItemCardapio { Nome = "Tortelli de Ricota", Descricao = "Tortelli de ricota ao molho de tomate", PrecoBase = 37.90m, Periodo = Periodo.Jantar, Ingredientes = [ing["Molho de Tomate"]] },
                new ItemCardapio { Nome = "Costela BBQ", Descricao = "Costela suÃ­na ao molho barbecue", PrecoBase = 44.90m, Periodo = Periodo.Jantar },
                new ItemCardapio { Nome = "Sopa de Cebola Gratinada", Descricao = "Sopa de cebola com queijo gratinado", PrecoBase = 28.90m, Periodo = Periodo.Jantar, Ingredientes = [ing["Cebola"], ing["Queijo Mussarela"]] },
                new ItemCardapio { Nome = "Bruschetta Caprese", Descricao = "Bruschetta com tomate, manjericÃ£o e queijo", PrecoBase = 26.90m, Periodo = Periodo.Jantar, Ingredientes = [ing["Tomate"], ing["Queijo Mussarela"]] },
                new ItemCardapio { Nome = "Carpaccio de SalmÃ£o", Descricao = "Carpaccio de salmÃ£o com alcaparras", PrecoBase = 39.90m, Periodo = Periodo.Jantar },
                new ItemCardapio { Nome = "Gnocchi ao Sugo", Descricao = "Gnocchi de batata ao molho sugo", PrecoBase = 34.90m, Periodo = Periodo.Jantar, Ingredientes = [ing["Batata"], ing["Molho de Tomate"]] },
                new ItemCardapio { Nome = "TiramisÃ¹", Descricao = "Sobremesa clÃ¡ssica italiana", PrecoBase = 22.90m, Periodo = Periodo.Jantar }
            });

            db.ItensCardapio.AddRange(itens);
            Console.WriteLine($"âœ“ {itens.Count} itens do cardÃ¡pio adicionados com sucesso (20 almoÃ§o + 20 jantar).");
        }
    }
}

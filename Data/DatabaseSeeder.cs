using A1_order_system.Data;
using A1_order_system.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace A1_order_system.Data
{
    public static class DatabaseSeeder
    {
        public static async Task SeedAsync(RestauranteDbContext db)
        {
            await db.Database.MigrateAsync();

            using var transaction = await db.Database.BeginTransactionAsync();

            try
            {
                await SeedUsuariosAsync(db);
                await SeedIngredientesAsync(db);
                await SeedMesasAsync(db);

                await db.SaveChangesAsync();  // Salva usuários, ingredientes e mesas

                await SeedItensCardapioAsync(db);  // Agora os ingredientes existem no banco!
                await db.SaveChangesAsync();  // Salva os itens do cardápio

                await transaction.CommitAsync();
                Console.WriteLine("✅ Database seeding concluído com sucesso!");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"❌ Erro no seeding: {ex.Message}");
                throw;
            }
        }

        private static async Task SeedUsuariosAsync(RestauranteDbContext db)
        {
            if (await db.Usuarios.AnyAsync()) return;

            var admin = new Usuario
            {
                Nome = "Administrador",
                Email = "admin@restaurante.com",
                Senha = BCrypt.Net.BCrypt.HashPassword("admin123")  // Senha com hash
            };

            db.Usuarios.Add(admin);
            await db.SaveChangesAsync();

            Console.WriteLine("✓ Usuário Admin criado (email: admin@restaurante.com | senha: admin123)");
        }

        private static async Task SeedIngredientesAsync(RestauranteDbContext db)
        {
            if (await db.Ingredientes.AnyAsync()) return;

            var ingredientes = new List<Ingrediente>
            {
                new Ingrediente { Nome = "Arroz Branco" },
                new Ingrediente { Nome = "Feijão Carioca" },
                new Ingrediente { Nome = "Carne de Alcatra" },
                new Ingrediente { Nome = "Frango Desossado" },
                new Ingrediente { Nome = "Tomate" },
                new Ingrediente { Nome = "Cebola" },
                new Ingrediente { Nome = "Alho" },
                new Ingrediente { Nome = "Óleo de Soja" },
                new Ingrediente { Nome = "Queijo Mussarela" },
                new Ingrediente { Nome = "Batata" },
                new Ingrediente { Nome = "Leite" },
                new Ingrediente { Nome = "Ovo" },
                new Ingrediente { Nome = "Macarrão Espaguete" },
                new Ingrediente { Nome = "Molho de Tomate" }
            };

            db.Ingredientes.AddRange(ingredientes);
            Console.WriteLine($"✓ {ingredientes.Count} ingredientes adicionados.");
        }

        private static async Task SeedMesasAsync(RestauranteDbContext db)
        {
            if (await db.Mesas.AnyAsync()) return;

            var mesas = new List<Mesa>
            {
                new Mesa { Numero = 1 },
                new Mesa { Numero = 2 },
                new Mesa { Numero = 3 },
                new Mesa { Numero = 4 },
                new Mesa { Numero = 5 },
                new Mesa { Numero = 6 },
                new Mesa { Numero = 7 },
                new Mesa { Numero = 8 }
            };

            db.Mesas.AddRange(mesas);
            Console.WriteLine($"✓ {mesas.Count} mesas criadas.");
        }

        private static async Task SeedItensCardapioAsync(RestauranteDbContext db)
        {
            if (await db.ItensCardapio.AnyAsync()) return;

            // Busca alguns ingredientes para associar
            var arroz = await db.Ingredientes.FirstAsync(i => i.Nome == "Arroz Branco");
            var feijao = await db.Ingredientes.FirstAsync(i => i.Nome == "Feijão Carioca");
            var carne = await db.Ingredientes.FirstAsync(i => i.Nome == "Carne de Alcatra");
            var tomate = await db.Ingredientes.FirstAsync(i => i.Nome == "Tomate");
            var queijo = await db.Ingredientes.FirstAsync(i => i.Nome == "Queijo Mussarela");

            var itens = new List<ItemCardapio>
            {
                new ItemCardapio
                {
                    Nome = "Arroz com Feijão e Bife",
                    Descricao = "Arroz branco, feijão carioca e bife grelhado",
                    PrecoBase = 32.90m,
                    Periodo = Periodo.Almoco,
                    Ingredientes = new List<Ingrediente> { arroz, feijao, carne }
                },
                new ItemCardapio
                {
                    Nome = "Lasanha à Bolonhesa",
                    Descricao = "Lasanha com molho bolonhesa e queijo gratinado",
                    PrecoBase = 39.90m,
                    Periodo = Periodo.Jantar,
                    Ingredientes = new List<Ingrediente> { tomate, queijo }
                },
                new ItemCardapio
                {
                    Nome = "Frango Grelhado com Arroz",
                    Descricao = "Filé de frango grelhado acompanhado de arroz",
                    PrecoBase = 28.90m,
                    Periodo = Periodo.Almoco,
                    Ingredientes = new List<Ingrediente> { arroz }
                },
                new ItemCardapio
                {
                    Nome = "Espaguete ao Molho",
                    Descricao = "Macarrão espaguete ao molho de tomate",
                    PrecoBase = 24.90m,
                    Periodo = Periodo.Jantar
                }
            };

            db.ItensCardapio.AddRange(itens);
            Console.WriteLine($"✓ {itens.Count} itens do cardápio adicionados.");
        }
    }
}
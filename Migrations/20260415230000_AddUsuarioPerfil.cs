using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace A1_order_system.Migrations
{
    public partial class AddUsuarioPerfil : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Perfil",
                table: "Usuarios",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Cliente");

            migrationBuilder.Sql(@"
                UPDATE Usuarios
                SET Perfil = 'Admin'
                WHERE Email = 'admin@restaurante.com';
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Perfil",
                table: "Usuarios");
        }
    }
}

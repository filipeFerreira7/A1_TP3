using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace A1_order_system.Migrations
{
    /// <inheritdoc />
    public partial class AddEnderecoIdToPedido : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "EnderecoId",
                table: "Pedidos",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pedidos_EnderecoId",
                table: "Pedidos",
                column: "EnderecoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Pedidos_Enderecos_EnderecoId",
                table: "Pedidos",
                column: "EnderecoId",
                principalTable: "Enderecos",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pedidos_Enderecos_EnderecoId",
                table: "Pedidos");

            migrationBuilder.DropIndex(
                name: "IX_Pedidos_EnderecoId",
                table: "Pedidos");

            migrationBuilder.DropColumn(
                name: "EnderecoId",
                table: "Pedidos");
        }
    }
}

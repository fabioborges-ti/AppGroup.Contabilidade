using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AppGroup.Contabilidade.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ContasContabeis",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    Codigo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Tipo = table.Column<int>(type: "int", maxLength: 50, nullable: false),
                    AceitaLancamentos = table.Column<bool>(type: "bit", nullable: false),
                    IdPai = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContasContabeis", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContasContabeis_ContasContabeis_IdPai",
                        column: x => x.IdPai,
                        principalTable: "ContasContabeis",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "ContasContabeis",
                columns: new[] { "Id", "AceitaLancamentos", "Codigo", "IdPai", "Nome", "Tipo" },
                values: new object[,]
                {
                    { new Guid("55ad855d-c9e8-4566-8ef0-fe189acc0533"), false, "1", null, "Receitas", 1 },
                    { new Guid("f1ceaafe-3d8f-4dbc-aac0-e1e27f8ec436"), false, "2", null, "Despesas", 2 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContasContabeis_IdPai",
                table: "ContasContabeis",
                column: "IdPai");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContasContabeis");
        }
    }
}

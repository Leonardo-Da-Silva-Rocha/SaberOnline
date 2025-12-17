using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SaberOnline.Faturamento.Data.Migrations
{
    /// <inheritdoc />
    public partial class fatInicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Pagamentos",
                columns: table => new
                {
                    PagamentoId = table.Column<Guid>(type: "UniqueIdentifier", nullable: false),
                    MatriculaId = table.Column<Guid>(type: "UniqueIdentifier", nullable: false),
                    Valor = table.Column<decimal>(type: "Money", precision: 10, scale: 2, nullable: false),
                    DataVencimento = table.Column<DateTime>(type: "SmallDateTime", nullable: false),
                    DataPagamento = table.Column<DateTime>(type: "SmallDateTime", nullable: true),
                    NumeroCartao = table.Column<string>(type: "Varchar", maxLength: 16, nullable: true, collation: "Latin1_General_CI_AI"),
                    NomeTitularCartao = table.Column<string>(type: "Varchar", maxLength: 50, nullable: true, collation: "Latin1_General_CI_AI"),
                    ValidadeCartao = table.Column<string>(type: "Varchar", maxLength: 5, nullable: true, collation: "Latin1_General_CI_AI"),
                    CVVCartao = table.Column<string>(type: "Varchar", maxLength: 3, nullable: true, collation: "Latin1_General_CI_AI"),
                    Status = table.Column<int>(type: "TinyInt", nullable: true),
                    CodigoConfirmacaoPagamento = table.Column<string>(type: "Varchar", maxLength: 100, nullable: true, collation: "Latin1_General_CI_AI")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PagamentosPK", x => x.PagamentoId);
                });

            migrationBuilder.CreateIndex(
                name: "PagamentoDataVencimentoIDX",
                table: "Pagamentos",
                column: "DataVencimento");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Pagamentos");
        }
    }
}

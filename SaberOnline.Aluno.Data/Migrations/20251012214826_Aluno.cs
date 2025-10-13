using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SaberOnline.Aluno.Data.Migrations
{
    /// <inheritdoc />
    public partial class Aluno : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Alunos",
                columns: table => new
                {
                    AlunoId = table.Column<Guid>(type: "UniqueIdentifier", nullable: false),
                    CodigoUsuarioAutenticacao = table.Column<Guid>(type: "UniqueIdentifier", nullable: false),
                    Nome = table.Column<string>(type: "Varchar", maxLength: 50, nullable: false, collation: "Latin1_General_CI_AI"),
                    Email = table.Column<string>(type: "Varchar", maxLength: 100, nullable: false, collation: "Latin1_General_CI_AI"),
                    DataNascimento = table.Column<DateTime>(type: "SmallDateTime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("AlunosPK", x => x.AlunoId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Alunos");
        }
    }
}

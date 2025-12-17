using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SaberOnline.Aluno.Data.Migrations
{
    /// <inheritdoc />
    public partial class AlunoMigration : Migration
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

            migrationBuilder.CreateTable(
                name: "MatriculasCursos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    AlunoId = table.Column<Guid>(type: "UniqueIdentifier", nullable: false),
                    CursoId = table.Column<Guid>(type: "TEXT", nullable: false),
                    NomeCurso = table.Column<string>(type: "TEXT", nullable: false),
                    Valor = table.Column<decimal>(type: "TEXT", nullable: false),
                    DataMatricula = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DataConclusao = table.Column<DateTime>(type: "TEXT", nullable: true),
                    EstadoMatricula = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MatriculasCursos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MatriculasCursos_Alunos_AlunoId",
                        column: x => x.AlunoId,
                        principalTable: "Alunos",
                        principalColumn: "AlunoId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Certificados",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    MatriculaCursoId = table.Column<Guid>(type: "TEXT", nullable: false),
                    DataSolicitacao = table.Column<DateTime>(type: "TEXT", nullable: false),
                    PathCertificado = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Certificados", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Certificados_MatriculasCursos_MatriculaCursoId",
                        column: x => x.MatriculaCursoId,
                        principalTable: "MatriculasCursos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HistoricoAprendizado",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    MatriculaCursoId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistoricoAprendizado", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HistoricoAprendizado_MatriculasCursos_MatriculaCursoId",
                        column: x => x.MatriculaCursoId,
                        principalTable: "MatriculasCursos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Certificados_MatriculaCursoId",
                table: "Certificados",
                column: "MatriculaCursoId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HistoricoAprendizado_MatriculaCursoId",
                table: "HistoricoAprendizado",
                column: "MatriculaCursoId");

            migrationBuilder.CreateIndex(
                name: "IX_MatriculasCursos_AlunoId",
                table: "MatriculasCursos",
                column: "AlunoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Certificados");

            migrationBuilder.DropTable(
                name: "HistoricoAprendizado");

            migrationBuilder.DropTable(
                name: "MatriculasCursos");

            migrationBuilder.DropTable(
                name: "Alunos");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eventos.Migrations
{
    /// <inheritdoc />
    public partial class RelacionamentosNN : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EventoCategorias",
                columns: table => new
                {
                    EventoId = table.Column<int>(type: "int", nullable: false),
                    CategoriaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventoCategorias", x => new { x.EventoId, x.CategoriaId });
                    table.ForeignKey(
                        name: "FK_EventoCategorias_Categoria_CategoriaId",
                        column: x => x.CategoriaId,
                        principalTable: "Categoria",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventoCategorias_Evento_EventoId",
                        column: x => x.EventoId,
                        principalTable: "Evento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EventoPalestrantes",
                columns: table => new
                {
                    EventoId = table.Column<int>(type: "int", nullable: false),
                    PalestranteId = table.Column<int>(type: "int", nullable: false),
                    Tema = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventoPalestrantes", x => new { x.EventoId, x.PalestranteId });
                    table.ForeignKey(
                        name: "FK_EventoPalestrantes_Evento_EventoId",
                        column: x => x.EventoId,
                        principalTable: "Evento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventoPalestrantes_Palestrante_PalestranteId",
                        column: x => x.PalestranteId,
                        principalTable: "Palestrante",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventoCategorias_CategoriaId",
                table: "EventoCategorias",
                column: "CategoriaId");

            migrationBuilder.CreateIndex(
                name: "IX_EventoPalestrantes_PalestranteId",
                table: "EventoPalestrantes",
                column: "PalestranteId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventoCategorias");

            migrationBuilder.DropTable(
                name: "EventoPalestrantes");
        }
    }
}

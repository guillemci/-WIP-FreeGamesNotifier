using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend_GameDiscountNotifier.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Plataformes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    NomPlataforma = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Plataformes", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SellersJocs",
                columns: table => new
                {
                    IdSeller = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    NomSeller = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SellersJocs", x => x.IdSeller);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Jocs",
                columns: table => new
                {
                    IdJoc = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IdSeller = table.Column<int>(type: "int", nullable: false),
                    Tipus = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Jocs", x => x.IdJoc);
                    table.ForeignKey(
                        name: "FK_Jocs_SellersJocs_IdSeller",
                        column: x => x.IdSeller,
                        principalTable: "SellersJocs",
                        principalColumn: "IdSeller",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "JocsEnPlataformes",
                columns: table => new
                {
                    IdJocPlatataforma = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    IdJoc = table.Column<int>(type: "int", nullable: false),
                    IdPlataforma = table.Column<int>(type: "int", nullable: false),
                    Desc = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Enllaç = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PreuOriginal = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    ImatgeLink = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JocsEnPlataformes", x => x.IdJocPlatataforma);
                    table.UniqueConstraint("AK_JocsEnPlataformes_IdJoc_IdPlataforma", x => new { x.IdJoc, x.IdPlataforma });
                    table.ForeignKey(
                        name: "FK_JocsEnPlataformes_Jocs_IdJoc",
                        column: x => x.IdJoc,
                        principalTable: "Jocs",
                        principalColumn: "IdJoc",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JocsEnPlataformes_Plataformes_IdPlataforma",
                        column: x => x.IdPlataforma,
                        principalTable: "Plataformes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Ofertas",
                columns: table => new
                {
                    IdExtretOferta = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IdJocPlatataforma = table.Column<int>(type: "int", nullable: false),
                    Descompte = table.Column<int>(type: "int", nullable: false),
                    DataIniciOferta = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    DataFiOferta = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    esGratis = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    PreuMomentOferta = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    DadesJsonOferta = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ofertas", x => x.IdExtretOferta);
                    table.ForeignKey(
                        name: "FK_Ofertas_JocsEnPlataformes_IdJocPlatataforma",
                        column: x => x.IdJocPlatataforma,
                        principalTable: "JocsEnPlataformes",
                        principalColumn: "IdJocPlatataforma",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Jocs_IdSeller",
                table: "Jocs",
                column: "IdSeller");

            migrationBuilder.CreateIndex(
                name: "IX_Jocs_Title",
                table: "Jocs",
                column: "Title");

            migrationBuilder.CreateIndex(
                name: "IX_JocsEnPlataformes_IdPlataforma",
                table: "JocsEnPlataformes",
                column: "IdPlataforma");

            migrationBuilder.CreateIndex(
                name: "IX_Ofertas_IdJocPlatataforma",
                table: "Ofertas",
                column: "IdJocPlatataforma");

            migrationBuilder.CreateIndex(
                name: "IX_Plataformes_NomPlataforma",
                table: "Plataformes",
                column: "NomPlataforma");

            migrationBuilder.CreateIndex(
                name: "IX_SellersJocs_NomSeller",
                table: "SellersJocs",
                column: "NomSeller");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Ofertas");

            migrationBuilder.DropTable(
                name: "JocsEnPlataformes");

            migrationBuilder.DropTable(
                name: "Jocs");

            migrationBuilder.DropTable(
                name: "Plataformes");

            migrationBuilder.DropTable(
                name: "SellersJocs");
        }
    }
}

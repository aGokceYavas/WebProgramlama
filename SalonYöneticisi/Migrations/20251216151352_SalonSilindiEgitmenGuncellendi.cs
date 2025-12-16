using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymManagementApp.Migrations
{
    /// <inheritdoc />
    public partial class SalonSilindiEgitmenGuncellendi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Egitmenler_Salonlar_SalonId",
                table: "Egitmenler");

            migrationBuilder.DropTable(
                name: "Salonlar");

            migrationBuilder.DropIndex(
                name: "IX_Egitmenler_SalonId",
                table: "Egitmenler");

            migrationBuilder.RenameColumn(
                name: "SalonId",
                table: "Egitmenler",
                newName: "BitisSaati");

            migrationBuilder.AddColumn<int>(
                name: "BaslamaSaati",
                table: "Egitmenler",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BaslamaSaati",
                table: "Egitmenler");

            migrationBuilder.RenameColumn(
                name: "BitisSaati",
                table: "Egitmenler",
                newName: "SalonId");

            migrationBuilder.CreateTable(
                name: "Salonlar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Adres = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Telefon = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Salonlar", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Egitmenler_SalonId",
                table: "Egitmenler",
                column: "SalonId");

            migrationBuilder.AddForeignKey(
                name: "FK_Egitmenler_Salonlar_SalonId",
                table: "Egitmenler",
                column: "SalonId",
                principalTable: "Salonlar",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

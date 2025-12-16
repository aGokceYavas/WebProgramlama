using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymManagementApp.Migrations
{
    /// <inheritdoc />
    public partial class PaketEgitmenBaglantisi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EgitmenId",
                table: "HizmetPaketleri",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_HizmetPaketleri_EgitmenId",
                table: "HizmetPaketleri",
                column: "EgitmenId");

            migrationBuilder.AddForeignKey(
                name: "FK_HizmetPaketleri_Egitmenler_EgitmenId",
                table: "HizmetPaketleri",
                column: "EgitmenId",
                principalTable: "Egitmenler",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HizmetPaketleri_Egitmenler_EgitmenId",
                table: "HizmetPaketleri");

            migrationBuilder.DropIndex(
                name: "IX_HizmetPaketleri_EgitmenId",
                table: "HizmetPaketleri");

            migrationBuilder.DropColumn(
                name: "EgitmenId",
                table: "HizmetPaketleri");
        }
    }
}

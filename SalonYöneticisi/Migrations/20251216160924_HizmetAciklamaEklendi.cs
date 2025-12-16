using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymManagementApp.Migrations
{
    /// <inheritdoc />
    public partial class HizmetAciklamaEklendi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Aciklama",
                table: "HizmetPaketleri",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Aciklama",
                table: "HizmetPaketleri");
        }
    }
}

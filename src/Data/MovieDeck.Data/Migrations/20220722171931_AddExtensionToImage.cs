using Microsoft.EntityFrameworkCore.Migrations;

namespace MovieDeck.Data.Migrations
{
    public partial class AddExtensionToImage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OriginalPath",
                table: "Images",
                newName: "RemoteImageUrl");

            migrationBuilder.AddColumn<string>(
                name: "Extension",
                table: "Images",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Extension",
                table: "Images");

            migrationBuilder.RenameColumn(
                name: "RemoteImageUrl",
                table: "Images",
                newName: "OriginalPath");
        }
    }
}

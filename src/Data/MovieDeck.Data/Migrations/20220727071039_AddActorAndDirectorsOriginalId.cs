using Microsoft.EntityFrameworkCore.Migrations;

namespace MovieDeck.Data.Migrations
{
    public partial class AddActorAndDirectorsOriginalId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OriginalId",
                table: "Directors",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OriginalId",
                table: "Actors",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OriginalId",
                table: "Directors");

            migrationBuilder.DropColumn(
                name: "OriginalId",
                table: "Actors");
        }
    }
}

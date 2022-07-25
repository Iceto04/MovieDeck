using Microsoft.EntityFrameworkCore.Migrations;

namespace MovieDeck.Data.Migrations
{
    public partial class AddMovieTrailer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TrailerKey",
                table: "Movies",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TrailerKey",
                table: "Movies");
        }
    }
}

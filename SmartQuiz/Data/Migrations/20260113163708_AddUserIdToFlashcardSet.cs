using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartQuiz.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdToFlashcardSet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "FlashcardSets",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "FlashcardSets");
        }
    }
}

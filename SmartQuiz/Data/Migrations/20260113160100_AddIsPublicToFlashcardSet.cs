using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartQuiz.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddIsPublicToFlashcardSet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "FlashcardSets",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "FlashcardSets");
        }
    }
}

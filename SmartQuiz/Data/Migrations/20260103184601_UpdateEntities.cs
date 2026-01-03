using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartQuiz.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BackImageUrl",
                table: "Flashcards");

            migrationBuilder.DropColumn(
                name: "CodeSnippet",
                table: "Flashcards");

            migrationBuilder.DropColumn(
                name: "DifficultyLevel",
                table: "Flashcards");

            migrationBuilder.RenameColumn(
                name: "FrontImageUrl",
                table: "Flashcards",
                newName: "ImageUrl");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Flashcards",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "FlashcardSetId",
                table: "Flashcards",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "FlashcardSets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlashcardSets", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Flashcards_FlashcardSetId",
                table: "Flashcards",
                column: "FlashcardSetId");

            migrationBuilder.AddForeignKey(
                name: "FK_Flashcards_FlashcardSets_FlashcardSetId",
                table: "Flashcards",
                column: "FlashcardSetId",
                principalTable: "FlashcardSets",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Flashcards_FlashcardSets_FlashcardSetId",
                table: "Flashcards");

            migrationBuilder.DropTable(
                name: "FlashcardSets");

            migrationBuilder.DropIndex(
                name: "IX_Flashcards_FlashcardSetId",
                table: "Flashcards");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Flashcards");

            migrationBuilder.DropColumn(
                name: "FlashcardSetId",
                table: "Flashcards");

            migrationBuilder.RenameColumn(
                name: "ImageUrl",
                table: "Flashcards",
                newName: "FrontImageUrl");

            migrationBuilder.AddColumn<string>(
                name: "BackImageUrl",
                table: "Flashcards",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CodeSnippet",
                table: "Flashcards",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DifficultyLevel",
                table: "Flashcards",
                type: "int",
                nullable: true);
        }
    }
}

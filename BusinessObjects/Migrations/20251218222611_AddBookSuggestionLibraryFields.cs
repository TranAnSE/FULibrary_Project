using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessObjects.Migrations
{
    /// <inheritdoc />
    public partial class AddBookSuggestionLibraryFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "LibraryId",
                table: "BookPurchaseSuggestions",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "ProcessedByLibrarian",
                table: "BookPurchaseSuggestions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProcessedByLibrarianId",
                table: "BookPurchaseSuggestions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BookPurchaseSuggestions_LibraryId",
                table: "BookPurchaseSuggestions",
                column: "LibraryId");

            migrationBuilder.CreateIndex(
                name: "IX_BookPurchaseSuggestions_ProcessedByLibrarian",
                table: "BookPurchaseSuggestions",
                column: "ProcessedByLibrarian");

            migrationBuilder.CreateIndex(
                name: "IX_BookPurchaseSuggestions_Status",
                table: "BookPurchaseSuggestions",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BookPurchaseSuggestions_LibraryId",
                table: "BookPurchaseSuggestions");

            migrationBuilder.DropIndex(
                name: "IX_BookPurchaseSuggestions_ProcessedByLibrarian",
                table: "BookPurchaseSuggestions");

            migrationBuilder.DropIndex(
                name: "IX_BookPurchaseSuggestions_Status",
                table: "BookPurchaseSuggestions");

            migrationBuilder.DropColumn(
                name: "LibraryId",
                table: "BookPurchaseSuggestions");

            migrationBuilder.DropColumn(
                name: "ProcessedByLibrarian",
                table: "BookPurchaseSuggestions");

            migrationBuilder.DropColumn(
                name: "ProcessedByLibrarianId",
                table: "BookPurchaseSuggestions");
        }
    }
}

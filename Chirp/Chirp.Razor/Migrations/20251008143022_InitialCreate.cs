using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chirp.Razor.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_message_user_AuthorId",
                table: "message");

            migrationBuilder.RenameColumn(
                name: "Username",
                table: "user",
                newName: "username");

            migrationBuilder.RenameColumn(
                name: "AuthorId",
                table: "user",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "Text",
                table: "message",
                newName: "text");

            migrationBuilder.RenameColumn(
                name: "TimeStamp",
                table: "message",
                newName: "pub_date");

            migrationBuilder.RenameColumn(
                name: "AuthorId",
                table: "message",
                newName: "author_id");

            migrationBuilder.RenameColumn(
                name: "CheepId",
                table: "message",
                newName: "message_id");

            migrationBuilder.RenameIndex(
                name: "IX_message_AuthorId",
                table: "message",
                newName: "IX_message_author_id");

            migrationBuilder.AddForeignKey(
                name: "FK_message_user_author_id",
                table: "message",
                column: "author_id",
                principalTable: "user",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_message_user_author_id",
                table: "message");

            migrationBuilder.RenameColumn(
                name: "username",
                table: "user",
                newName: "Username");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "user",
                newName: "AuthorId");

            migrationBuilder.RenameColumn(
                name: "text",
                table: "message",
                newName: "Text");

            migrationBuilder.RenameColumn(
                name: "pub_date",
                table: "message",
                newName: "TimeStamp");

            migrationBuilder.RenameColumn(
                name: "author_id",
                table: "message",
                newName: "AuthorId");

            migrationBuilder.RenameColumn(
                name: "message_id",
                table: "message",
                newName: "CheepId");

            migrationBuilder.RenameIndex(
                name: "IX_message_author_id",
                table: "message",
                newName: "IX_message_AuthorId");

            migrationBuilder.AddForeignKey(
                name: "FK_message_user_AuthorId",
                table: "message",
                column: "AuthorId",
                principalTable: "user",
                principalColumn: "AuthorId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

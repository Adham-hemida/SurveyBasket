using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SurveyBasket.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditColumnsToPollsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "Polls",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "Polls",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "UpdatedById",
                table: "Polls",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedOn",
                table: "Polls",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Polls_CreatedById",
                table: "Polls",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Polls_UpdatedById",
                table: "Polls",
                column: "UpdatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Polls_AspNetUsers_CreatedById",
                table: "Polls",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Polls_AspNetUsers_UpdatedById",
                table: "Polls",
                column: "UpdatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Polls_AspNetUsers_CreatedById",
                table: "Polls");

            migrationBuilder.DropForeignKey(
                name: "FK_Polls_AspNetUsers_UpdatedById",
                table: "Polls");

            migrationBuilder.DropIndex(
                name: "IX_Polls_CreatedById",
                table: "Polls");

            migrationBuilder.DropIndex(
                name: "IX_Polls_UpdatedById",
                table: "Polls");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Polls");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "Polls");

            migrationBuilder.DropColumn(
                name: "UpdatedById",
                table: "Polls");

            migrationBuilder.DropColumn(
                name: "UpdatedOn",
                table: "Polls");
        }
    }
}

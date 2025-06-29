using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeInTasks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Users_CreatedBy",
                table: "Tasks");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Users_UpdatedBy",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_CreatedBy",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_UpdatedBy",
                table: "Tasks");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Tasks_CreatedBy",
                table: "Tasks",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_UpdatedBy",
                table: "Tasks",
                column: "UpdatedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Users_CreatedBy",
                table: "Tasks",
                column: "CreatedBy",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Users_UpdatedBy",
                table: "Tasks",
                column: "UpdatedBy",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

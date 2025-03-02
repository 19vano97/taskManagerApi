using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskManagerApi.Migrations
{
    /// <inheritdoc />
    public partial class uniqueFalseForStatusId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TaskItems_StatusId",
                table: "TaskItems");

            migrationBuilder.CreateIndex(
                name: "IX_TaskItems_StatusId",
                table: "TaskItems",
                column: "StatusId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TaskItems_StatusId",
                table: "TaskItems");

            migrationBuilder.CreateIndex(
                name: "IX_TaskItems_StatusId",
                table: "TaskItems",
                column: "StatusId",
                unique: true);
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskManagerApi.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedProjectToOrg4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationAccount_OrganizationItem_OrganizationIdId",
                table: "OrganizationAccount");

            migrationBuilder.RenameColumn(
                name: "OrganizationIdId",
                table: "OrganizationAccount",
                newName: "OrganizationId");

            migrationBuilder.RenameIndex(
                name: "IX_OrganizationAccount_OrganizationIdId",
                table: "OrganizationAccount",
                newName: "IX_OrganizationAccount_OrganizationId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationAccount_OrganizationItem_OrganizationId",
                table: "OrganizationAccount",
                column: "OrganizationId",
                principalTable: "OrganizationItem",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationAccount_OrganizationItem_OrganizationId",
                table: "OrganizationAccount");

            migrationBuilder.RenameColumn(
                name: "OrganizationId",
                table: "OrganizationAccount",
                newName: "OrganizationIdId");

            migrationBuilder.RenameIndex(
                name: "IX_OrganizationAccount_OrganizationId",
                table: "OrganizationAccount",
                newName: "IX_OrganizationAccount_OrganizationIdId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationAccount_OrganizationItem_OrganizationIdId",
                table: "OrganizationAccount",
                column: "OrganizationIdId",
                principalTable: "OrganizationItem",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

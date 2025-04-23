using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskManagerApi.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedProjectToOrg : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectItems_OrganizationItem_OrganizationId",
                table: "ProjectItems");

            migrationBuilder.RenameColumn(
                name: "OrganizationId",
                table: "ProjectItems",
                newName: "OrganizationIdId");

            migrationBuilder.RenameIndex(
                name: "IX_ProjectItems_OrganizationId",
                table: "ProjectItems",
                newName: "IX_ProjectItems_OrganizationIdId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectItems_OrganizationItem_OrganizationIdId",
                table: "ProjectItems",
                column: "OrganizationIdId",
                principalTable: "OrganizationItem",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectItems_OrganizationItem_OrganizationIdId",
                table: "ProjectItems");

            migrationBuilder.RenameColumn(
                name: "OrganizationIdId",
                table: "ProjectItems",
                newName: "OrganizationId");

            migrationBuilder.RenameIndex(
                name: "IX_ProjectItems_OrganizationIdId",
                table: "ProjectItems",
                newName: "IX_ProjectItems_OrganizationId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectItems_OrganizationItem_OrganizationId",
                table: "ProjectItems",
                column: "OrganizationId",
                principalTable: "OrganizationItem",
                principalColumn: "Id");
        }
    }
}

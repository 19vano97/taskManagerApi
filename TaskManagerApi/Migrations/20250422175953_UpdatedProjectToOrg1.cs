using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskManagerApi.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedProjectToOrg1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectItems_OrganizationItem_OrganizationIdId",
                table: "ProjectItems");

            migrationBuilder.DropIndex(
                name: "IX_ProjectItems_OrganizationIdId",
                table: "ProjectItems");

            migrationBuilder.DropColumn(
                name: "OrganizationIdId",
                table: "ProjectItems");

            migrationBuilder.AddColumn<Guid>(
                name: "OrganizationId",
                table: "ProjectItems",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_ProjectItems_OrganizationId",
                table: "ProjectItems",
                column: "OrganizationId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectItems_OrganizationItem_OrganizationId",
                table: "ProjectItems",
                column: "OrganizationId",
                principalTable: "OrganizationItem",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectItems_OrganizationItem_OrganizationId",
                table: "ProjectItems");

            migrationBuilder.DropIndex(
                name: "IX_ProjectItems_OrganizationId",
                table: "ProjectItems");

            migrationBuilder.DropColumn(
                name: "OrganizationId",
                table: "ProjectItems");

            migrationBuilder.AddColumn<Guid>(
                name: "OrganizationIdId",
                table: "ProjectItems",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectItems_OrganizationIdId",
                table: "ProjectItems",
                column: "OrganizationIdId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectItems_OrganizationItem_OrganizationIdId",
                table: "ProjectItems",
                column: "OrganizationIdId",
                principalTable: "OrganizationItem",
                principalColumn: "Id");
        }
    }
}

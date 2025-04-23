using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskManagerApi.Migrations
{
    /// <inheritdoc />
    public partial class AddOrganizations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "OrganizationId",
                table: "ProjectItems",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "OrginizationId",
                table: "ProjectItems",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "OrganizationItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Abbreviation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Owner = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationItem", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectItems_OrganizationId",
                table: "ProjectItems",
                column: "OrganizationId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectItems_OrganizationItem_OrganizationId",
                table: "ProjectItems",
                column: "OrganizationId",
                principalTable: "OrganizationItem",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectItems_OrganizationItem_OrganizationId",
                table: "ProjectItems");

            migrationBuilder.DropTable(
                name: "OrganizationItem");

            migrationBuilder.DropIndex(
                name: "IX_ProjectItems_OrganizationId",
                table: "ProjectItems");

            migrationBuilder.DropColumn(
                name: "OrganizationId",
                table: "ProjectItems");

            migrationBuilder.DropColumn(
                name: "OrginizationId",
                table: "ProjectItems");
        }
    }
}

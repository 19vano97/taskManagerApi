using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskManagerApi.Migrations
{
    /// <inheritdoc />
    public partial class AddedStatusProjectMap : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskItems_TaskItemStatuses_StatusId",
                table: "TaskItems");

            migrationBuilder.AlterColumn<int>(
                name: "StatusId",
                table: "TaskItems",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateTable(
                name: "ProjectTaskStatusMapping",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectTaskStatusMapping", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectTaskStatusMapping_ProjectItems_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "ProjectItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectTaskStatusMapping_TaskItemStatuses_StatusId",
                        column: x => x.StatusId,
                        principalTable: "TaskItemStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTaskStatusMapping_ProjectId",
                table: "ProjectTaskStatusMapping",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTaskStatusMapping_StatusId",
                table: "ProjectTaskStatusMapping",
                column: "StatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskItems_TaskItemStatuses_StatusId",
                table: "TaskItems",
                column: "StatusId",
                principalTable: "TaskItemStatuses",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskItems_TaskItemStatuses_StatusId",
                table: "TaskItems");

            migrationBuilder.DropTable(
                name: "ProjectTaskStatusMapping");

            migrationBuilder.AlterColumn<int>(
                name: "StatusId",
                table: "TaskItems",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskItems_TaskItemStatuses_StatusId",
                table: "TaskItems",
                column: "StatusId",
                principalTable: "TaskItemStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

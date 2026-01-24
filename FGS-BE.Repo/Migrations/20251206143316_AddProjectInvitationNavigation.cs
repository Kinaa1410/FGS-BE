using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FGS_BE.Repo.Migrations
{
    /// <inheritdoc />
    public partial class AddProjectInvitationNavigation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Projects",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Projects",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Open",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Projects",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CurrentMembers",
                table: "Projects",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MaxMembers",
                table: "Projects",
                type: "int",
                nullable: false,
                defaultValue: 10);

            migrationBuilder.AddColumn<int>(
                name: "MinMembers",
                table: "Projects",
                type: "int",
                nullable: false,
                defaultValue: 2);

            migrationBuilder.CreateTable(
                name: "ProjectInvitation",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProjectId = table.Column<int>(type: "int", nullable: false),
                    InviterId = table.Column<int>(type: "int", nullable: false),
                    InvitedUserId = table.Column<int>(type: "int", nullable: true),
                    InviteCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiryAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectInvitation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectInvitation_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectInvitation_Users_InvitedUserId",
                        column: x => x.InvitedUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProjectInvitation_Users_InviterId",
                        column: x => x.InviterId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Projects_CurrentMembers",
                table: "Projects",
                column: "CurrentMembers");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectInvitation_InvitedUserId",
                table: "ProjectInvitation",
                column: "InvitedUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectInvitation_InviterId",
                table: "ProjectInvitation",
                column: "InviterId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectInvitation_ProjectId",
                table: "ProjectInvitation",
                column: "ProjectId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProjectInvitation");

            migrationBuilder.DropIndex(
                name: "IX_Projects_CurrentMembers",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "CurrentMembers",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "MaxMembers",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "MinMembers",
                table: "Projects");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Projects",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Projects",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldDefaultValue: "Open");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Projects",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}

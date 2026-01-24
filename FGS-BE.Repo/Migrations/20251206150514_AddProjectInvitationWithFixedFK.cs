using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FGS_BE.Repo.Migrations
{
    /// <inheritdoc />
    public partial class AddProjectInvitationWithFixedFK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ReservedMembers",
                table: "Projects",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "ProjectInvitation",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ExpiryAt",
                table: "ProjectInvitation",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Projects_ReservedMembers",
                table: "Projects",
                column: "ReservedMembers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Projects_ReservedMembers",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "ReservedMembers",
                table: "Projects");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "ProjectInvitation",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ExpiryAt",
                table: "ProjectInvitation",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FGS_BE.Repo.Migrations
{
    /// <inheritdoc />
    public partial class FinalFixDuplicates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatMessages_ChatRooms_ChatRoomId1",
                table: "ChatMessages");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_NotificationTemplates_NotificationTemplateId1",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Users_AssigneeId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_NotificationTemplateId1",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_ChatMessages_ChatRoomId1",
                table: "ChatMessages");

            migrationBuilder.DropColumn(
                name: "NotificationTemplateId1",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "ChatRoomId1",
                table: "ChatMessages");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ProcessedAt",
                table: "RedeemRequests",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<int>(
                name: "ProjectId",
                table: "ChatRooms",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Users_AssigneeId",
                table: "Tasks",
                column: "AssigneeId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Users_AssigneeId",
                table: "Tasks");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ProcessedAt",
                table: "RedeemRequests",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NotificationTemplateId1",
                table: "Notifications",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ProjectId",
                table: "ChatRooms",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ChatRoomId1",
                table: "ChatMessages",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_NotificationTemplateId1",
                table: "Notifications",
                column: "NotificationTemplateId1");

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_ChatRoomId1",
                table: "ChatMessages",
                column: "ChatRoomId1");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatMessages_ChatRooms_ChatRoomId1",
                table: "ChatMessages",
                column: "ChatRoomId1",
                principalTable: "ChatRooms",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_NotificationTemplates_NotificationTemplateId1",
                table: "Notifications",
                column: "NotificationTemplateId1",
                principalTable: "NotificationTemplates",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Users_AssigneeId",
                table: "Tasks",
                column: "AssigneeId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}

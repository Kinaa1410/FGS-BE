using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FGS_BE.Repo.Migrations
{
    /// <inheritdoc />
    public partial class AddMentor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectInvitation_Projects_ProjectId",
                table: "ProjectInvitation");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectInvitation_Users_InvitedUserId",
                table: "ProjectInvitation");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectInvitation_Users_InviterId",
                table: "ProjectInvitation");

            migrationBuilder.DropForeignKey(
                name: "FK_UserLevel_Level_LevelId",
                table: "UserLevel");

            migrationBuilder.DropForeignKey(
                name: "FK_UserLevel_Users_UserId",
                table: "UserLevel");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserLevel",
                table: "UserLevel");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProjectInvitation",
                table: "ProjectInvitation");

            migrationBuilder.RenameTable(
                name: "UserLevel",
                newName: "UserLevels");

            migrationBuilder.RenameTable(
                name: "ProjectInvitation",
                newName: "ProjectInvitations");

            migrationBuilder.RenameIndex(
                name: "IX_UserLevel_UserId",
                table: "UserLevels",
                newName: "IX_UserLevels_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserLevel_LevelId",
                table: "UserLevels",
                newName: "IX_UserLevels_LevelId");

            migrationBuilder.RenameIndex(
                name: "IX_ProjectInvitation_ProjectId",
                table: "ProjectInvitations",
                newName: "IX_ProjectInvitations_ProjectId");

            migrationBuilder.RenameIndex(
                name: "IX_ProjectInvitation_InviterId",
                table: "ProjectInvitations",
                newName: "IX_ProjectInvitations_InviterId");

            migrationBuilder.RenameIndex(
                name: "IX_ProjectInvitation_InvitedUserId",
                table: "ProjectInvitations",
                newName: "IX_ProjectInvitations_InvitedUserId");

            migrationBuilder.AddColumn<int>(
                name: "MentorId",
                table: "Projects",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "ProjectInvitations",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "pending",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Message",
                table: "ProjectInvitations",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "InviteCode",
                table: "ProjectInvitations",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ExpiryAt",
                table: "ProjectInvitations",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "DATEADD(MINUTE, 15, GETUTCDATE())",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "ProjectInvitations",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserLevels",
                table: "UserLevels",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProjectInvitations",
                table: "ProjectInvitations",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_MentorId",
                table: "Projects",
                column: "MentorId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectInvitations_ExpiryAt",
                table: "ProjectInvitations",
                column: "ExpiryAt");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectInvitations_InviteCode",
                table: "ProjectInvitations",
                column: "InviteCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectInvitations_Status",
                table: "ProjectInvitations",
                column: "Status");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectInvitations_Projects_ProjectId",
                table: "ProjectInvitations",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectInvitations_Users_InvitedUserId",
                table: "ProjectInvitations",
                column: "InvitedUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectInvitations_Users_InviterId",
                table: "ProjectInvitations",
                column: "InviterId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_Users_MentorId",
                table: "Projects",
                column: "MentorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_UserLevels_Level_LevelId",
                table: "UserLevels",
                column: "LevelId",
                principalTable: "Level",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserLevels_Users_UserId",
                table: "UserLevels",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectInvitations_Projects_ProjectId",
                table: "ProjectInvitations");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectInvitations_Users_InvitedUserId",
                table: "ProjectInvitations");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectInvitations_Users_InviterId",
                table: "ProjectInvitations");

            migrationBuilder.DropForeignKey(
                name: "FK_Projects_Users_MentorId",
                table: "Projects");

            migrationBuilder.DropForeignKey(
                name: "FK_UserLevels_Level_LevelId",
                table: "UserLevels");

            migrationBuilder.DropForeignKey(
                name: "FK_UserLevels_Users_UserId",
                table: "UserLevels");

            migrationBuilder.DropIndex(
                name: "IX_Projects_MentorId",
                table: "Projects");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserLevels",
                table: "UserLevels");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProjectInvitations",
                table: "ProjectInvitations");

            migrationBuilder.DropIndex(
                name: "IX_ProjectInvitations_ExpiryAt",
                table: "ProjectInvitations");

            migrationBuilder.DropIndex(
                name: "IX_ProjectInvitations_InviteCode",
                table: "ProjectInvitations");

            migrationBuilder.DropIndex(
                name: "IX_ProjectInvitations_Status",
                table: "ProjectInvitations");

            migrationBuilder.DropColumn(
                name: "MentorId",
                table: "Projects");

            migrationBuilder.RenameTable(
                name: "UserLevels",
                newName: "UserLevel");

            migrationBuilder.RenameTable(
                name: "ProjectInvitations",
                newName: "ProjectInvitation");

            migrationBuilder.RenameIndex(
                name: "IX_UserLevels_UserId",
                table: "UserLevel",
                newName: "IX_UserLevel_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserLevels_LevelId",
                table: "UserLevel",
                newName: "IX_UserLevel_LevelId");

            migrationBuilder.RenameIndex(
                name: "IX_ProjectInvitations_ProjectId",
                table: "ProjectInvitation",
                newName: "IX_ProjectInvitation_ProjectId");

            migrationBuilder.RenameIndex(
                name: "IX_ProjectInvitations_InviterId",
                table: "ProjectInvitation",
                newName: "IX_ProjectInvitation_InviterId");

            migrationBuilder.RenameIndex(
                name: "IX_ProjectInvitations_InvitedUserId",
                table: "ProjectInvitation",
                newName: "IX_ProjectInvitation_InvitedUserId");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "ProjectInvitation",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldDefaultValue: "pending");

            migrationBuilder.AlterColumn<string>(
                name: "Message",
                table: "ProjectInvitation",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "InviteCode",
                table: "ProjectInvitation",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ExpiryAt",
                table: "ProjectInvitation",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "DATEADD(MINUTE, 15, GETUTCDATE())");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "ProjectInvitation",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserLevel",
                table: "UserLevel",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProjectInvitation",
                table: "ProjectInvitation",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectInvitation_Projects_ProjectId",
                table: "ProjectInvitation",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectInvitation_Users_InvitedUserId",
                table: "ProjectInvitation",
                column: "InvitedUserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectInvitation_Users_InviterId",
                table: "ProjectInvitation",
                column: "InviterId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserLevel_Level_LevelId",
                table: "UserLevel",
                column: "LevelId",
                principalTable: "Level",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserLevel_Users_UserId",
                table: "UserLevel",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

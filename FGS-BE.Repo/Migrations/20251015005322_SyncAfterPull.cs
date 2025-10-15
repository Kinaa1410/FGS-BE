using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FGS_BE.Repo.Migrations
{
    /// <inheritdoc />
    public partial class SyncAfterPull : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatMessages_ChatRooms_ChatRoomId1",
                table: "ChatMessages");

            migrationBuilder.DropForeignKey(
                name: "FK_ChatMessages_Users_UserId",
                table: "ChatMessages");

            migrationBuilder.DropForeignKey(
                name: "FK_ChatParticipants_Users_UserId1",
                table: "ChatParticipants");

            migrationBuilder.DropForeignKey(
                name: "FK_ChatRooms_Users_UserId",
                table: "ChatRooms");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_NotificationTemplates_NotificationTemplateId1",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Users_UserId1",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_PerformanceScores_Milestones_MilestoneId1",
                table: "PerformanceScores");

            migrationBuilder.DropForeignKey(
                name: "FK_PerformanceScores_Projects_ProjectId1",
                table: "PerformanceScores");

            migrationBuilder.DropForeignKey(
                name: "FK_PerformanceScores_Tasks_TaskId1",
                table: "PerformanceScores");

            migrationBuilder.DropForeignKey(
                name: "FK_PerformanceScores_Users_UserId1",
                table: "PerformanceScores");

            migrationBuilder.DropForeignKey(
                name: "FK_Projects_Semesters_SemesterId1",
                table: "Projects");

            migrationBuilder.DropForeignKey(
                name: "FK_RedeemRequests_Users_UserId1",
                table: "RedeemRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_Submissions_Users_UserId1",
                table: "Submissions");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Users_AssigneeId",
                table: "Tasks");

            migrationBuilder.DropForeignKey(
                name: "FK_TermKeywords_Semesters_SemesterId1",
                table: "TermKeywords");

            migrationBuilder.DropForeignKey(
                name: "FK_UserAchievements_Achievements_AchievementId",
                table: "UserAchievements");

            migrationBuilder.DropForeignKey(
                name: "FK_UserAchievements_Users_UserId",
                table: "UserAchievements");

            migrationBuilder.DropIndex(
                name: "IX_TermKeywords_SemesterId1",
                table: "TermKeywords");

            migrationBuilder.DropIndex(
                name: "IX_Submissions_UserId1",
                table: "Submissions");

            migrationBuilder.DropIndex(
                name: "IX_RedeemRequests_UserId1",
                table: "RedeemRequests");

            migrationBuilder.DropIndex(
                name: "IX_Projects_SemesterId1",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_PerformanceScores_MilestoneId1",
                table: "PerformanceScores");

            migrationBuilder.DropIndex(
                name: "IX_PerformanceScores_ProjectId1",
                table: "PerformanceScores");

            migrationBuilder.DropIndex(
                name: "IX_PerformanceScores_TaskId1",
                table: "PerformanceScores");

            migrationBuilder.DropIndex(
                name: "IX_PerformanceScores_UserId1",
                table: "PerformanceScores");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_NotificationTemplateId1",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_UserId1",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_ChatParticipants_UserId1",
                table: "ChatParticipants");

            migrationBuilder.DropIndex(
                name: "IX_ChatMessages_ChatRoomId1",
                table: "ChatMessages");

            migrationBuilder.DropIndex(
                name: "IX_ChatMessages_UserId",
                table: "ChatMessages");

            migrationBuilder.DropColumn(
                name: "SemesterId1",
                table: "TermKeywords");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "Submissions");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "RedeemRequests");

            migrationBuilder.DropColumn(
                name: "SemesterId1",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "MilestoneId1",
                table: "PerformanceScores");

            migrationBuilder.DropColumn(
                name: "ProjectId1",
                table: "PerformanceScores");

            migrationBuilder.DropColumn(
                name: "TaskId1",
                table: "PerformanceScores");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "PerformanceScores");

            migrationBuilder.DropColumn(
                name: "NotificationTemplateId1",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "ChatParticipants");

            migrationBuilder.DropColumn(
                name: "ChatRoomId1",
                table: "ChatMessages");

            migrationBuilder.DropColumn(
                name: "UserId",
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
                name: "FK_ChatRooms_Users_UserId",
                table: "ChatRooms",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Users_AssigneeId",
                table: "Tasks",
                column: "AssigneeId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserAchievements_Achievements_AchievementId",
                table: "UserAchievements",
                column: "AchievementId",
                principalTable: "Achievements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserAchievements_Users_UserId",
                table: "UserAchievements",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatRooms_Users_UserId",
                table: "ChatRooms");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Users_AssigneeId",
                table: "Tasks");

            migrationBuilder.DropForeignKey(
                name: "FK_UserAchievements_Achievements_AchievementId",
                table: "UserAchievements");

            migrationBuilder.DropForeignKey(
                name: "FK_UserAchievements_Users_UserId",
                table: "UserAchievements");

            migrationBuilder.AddColumn<int>(
                name: "SemesterId1",
                table: "TermKeywords",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserId1",
                table: "Submissions",
                type: "int",
                nullable: true);

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
                name: "UserId1",
                table: "RedeemRequests",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SemesterId1",
                table: "Projects",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MilestoneId1",
                table: "PerformanceScores",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProjectId1",
                table: "PerformanceScores",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TaskId1",
                table: "PerformanceScores",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserId1",
                table: "PerformanceScores",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NotificationTemplateId1",
                table: "Notifications",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserId1",
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
                name: "UserId1",
                table: "ChatParticipants",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ChatRoomId1",
                table: "ChatMessages",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "ChatMessages",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TermKeywords_SemesterId1",
                table: "TermKeywords",
                column: "SemesterId1");

            migrationBuilder.CreateIndex(
                name: "IX_Submissions_UserId1",
                table: "Submissions",
                column: "UserId1");

            migrationBuilder.CreateIndex(
                name: "IX_RedeemRequests_UserId1",
                table: "RedeemRequests",
                column: "UserId1");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_SemesterId1",
                table: "Projects",
                column: "SemesterId1");

            migrationBuilder.CreateIndex(
                name: "IX_PerformanceScores_MilestoneId1",
                table: "PerformanceScores",
                column: "MilestoneId1");

            migrationBuilder.CreateIndex(
                name: "IX_PerformanceScores_ProjectId1",
                table: "PerformanceScores",
                column: "ProjectId1");

            migrationBuilder.CreateIndex(
                name: "IX_PerformanceScores_TaskId1",
                table: "PerformanceScores",
                column: "TaskId1");

            migrationBuilder.CreateIndex(
                name: "IX_PerformanceScores_UserId1",
                table: "PerformanceScores",
                column: "UserId1");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_NotificationTemplateId1",
                table: "Notifications",
                column: "NotificationTemplateId1");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId1",
                table: "Notifications",
                column: "UserId1");

            migrationBuilder.CreateIndex(
                name: "IX_ChatParticipants_UserId1",
                table: "ChatParticipants",
                column: "UserId1");

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_ChatRoomId1",
                table: "ChatMessages",
                column: "ChatRoomId1");

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_UserId",
                table: "ChatMessages",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatMessages_ChatRooms_ChatRoomId1",
                table: "ChatMessages",
                column: "ChatRoomId1",
                principalTable: "ChatRooms",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatMessages_Users_UserId",
                table: "ChatMessages",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatParticipants_Users_UserId1",
                table: "ChatParticipants",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatRooms_Users_UserId",
                table: "ChatRooms",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_NotificationTemplates_NotificationTemplateId1",
                table: "Notifications",
                column: "NotificationTemplateId1",
                principalTable: "NotificationTemplates",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Users_UserId1",
                table: "Notifications",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PerformanceScores_Milestones_MilestoneId1",
                table: "PerformanceScores",
                column: "MilestoneId1",
                principalTable: "Milestones",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PerformanceScores_Projects_ProjectId1",
                table: "PerformanceScores",
                column: "ProjectId1",
                principalTable: "Projects",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PerformanceScores_Tasks_TaskId1",
                table: "PerformanceScores",
                column: "TaskId1",
                principalTable: "Tasks",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PerformanceScores_Users_UserId1",
                table: "PerformanceScores",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_Semesters_SemesterId1",
                table: "Projects",
                column: "SemesterId1",
                principalTable: "Semesters",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RedeemRequests_Users_UserId1",
                table: "RedeemRequests",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Submissions_Users_UserId1",
                table: "Submissions",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Users_AssigneeId",
                table: "Tasks",
                column: "AssigneeId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TermKeywords_Semesters_SemesterId1",
                table: "TermKeywords",
                column: "SemesterId1",
                principalTable: "Semesters",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserAchievements_Achievements_AchievementId",
                table: "UserAchievements",
                column: "AchievementId",
                principalTable: "Achievements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserAchievements_Users_UserId",
                table: "UserAchievements",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

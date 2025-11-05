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
            // Conditional drops for FKs (skip if don't exist)
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ChatMessages_ChatRooms_ChatRoomId1')
                    ALTER TABLE [ChatMessages] DROP CONSTRAINT [FK_ChatMessages_ChatRooms_ChatRoomId1];
            ");
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ChatMessages_Users_UserId')
                    ALTER TABLE [ChatMessages] DROP CONSTRAINT [FK_ChatMessages_Users_UserId];
            ");
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ChatParticipants_Users_UserId1')
                    ALTER TABLE [ChatParticipants] DROP CONSTRAINT [FK_ChatParticipants_Users_UserId1];
            ");
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ChatRooms_Users_UserId')
                    ALTER TABLE [ChatRooms] DROP CONSTRAINT [FK_ChatRooms_Users_UserId];
            ");
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Notifications_NotificationTemplates_NotificationTemplateId1')
                    ALTER TABLE [Notifications] DROP CONSTRAINT [FK_Notifications_NotificationTemplates_NotificationTemplateId1];
            ");
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Notifications_Users_UserId1')
                    ALTER TABLE [Notifications] DROP CONSTRAINT [FK_Notifications_Users_UserId1];
            ");
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_PerformanceScores_Milestones_MilestoneId1')
                    ALTER TABLE [PerformanceScores] DROP CONSTRAINT [FK_PerformanceScores_Milestones_MilestoneId1];
            ");
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_PerformanceScores_Projects_ProjectId1')
                    ALTER TABLE [PerformanceScores] DROP CONSTRAINT [FK_PerformanceScores_Projects_ProjectId1];
            ");
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_PerformanceScores_Tasks_TaskId1')
                    ALTER TABLE [PerformanceScores] DROP CONSTRAINT [FK_PerformanceScores_Tasks_TaskId1];
            ");
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_PerformanceScores_Users_UserId1')
                    ALTER TABLE [PerformanceScores] DROP CONSTRAINT [FK_PerformanceScores_Users_UserId1];
            ");
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Projects_Semesters_SemesterId1')
                    ALTER TABLE [Projects] DROP CONSTRAINT [FK_Projects_Semesters_SemesterId1];
            ");
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_RedeemRequests_Users_UserId1')
                    ALTER TABLE [RedeemRequests] DROP CONSTRAINT [FK_RedeemRequests_Users_UserId1];
            ");
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Submissions_Users_UserId1')
                    ALTER TABLE [Submissions] DROP CONSTRAINT [FK_Submissions_Users_UserId1];
            ");
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Tasks_Users_AssigneeId')
                    ALTER TABLE [Tasks] DROP CONSTRAINT [FK_Tasks_Users_AssigneeId];
            ");
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_TermKeywords_Semesters_SemesterId1')
                    ALTER TABLE [TermKeywords] DROP CONSTRAINT [FK_TermKeywords_Semesters_SemesterId1];
            ");
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_UserAchievements_Achievements_AchievementId')
                    ALTER TABLE [UserAchievements] DROP CONSTRAINT [FK_UserAchievements_Achievements_AchievementId];
            ");
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_UserAchievements_Users_UserId')
                    ALTER TABLE [UserAchievements] DROP CONSTRAINT [FK_UserAchievements_Users_UserId];
            ");

            // Conditional drops for Indexes
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_TermKeywords_SemesterId1')
                    DROP INDEX [IX_TermKeywords_SemesterId1] ON [TermKeywords];
            ");
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Submissions_UserId1')
                    DROP INDEX [IX_Submissions_UserId1] ON [Submissions];
            ");
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_RedeemRequests_UserId1')
                    DROP INDEX [IX_RedeemRequests_UserId1] ON [RedeemRequests];
            ");
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Projects_SemesterId1')
                    DROP INDEX [IX_Projects_SemesterId1] ON [Projects];
            ");
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_PerformanceScores_MilestoneId1')
                    DROP INDEX [IX_PerformanceScores_MilestoneId1] ON [PerformanceScores];
            ");
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_PerformanceScores_ProjectId1')
                    DROP INDEX [IX_PerformanceScores_ProjectId1] ON [PerformanceScores];
            ");
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_PerformanceScores_TaskId1')
                    DROP INDEX [IX_PerformanceScores_TaskId1] ON [PerformanceScores];
            ");
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_PerformanceScores_UserId1')
                    DROP INDEX [IX_PerformanceScores_UserId1] ON [PerformanceScores];
            ");
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Notifications_NotificationTemplateId1')
                    DROP INDEX [IX_Notifications_NotificationTemplateId1] ON [Notifications];
            ");
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Notifications_UserId1')
                    DROP INDEX [IX_Notifications_UserId1] ON [Notifications];
            ");
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ChatParticipants_UserId1')
                    DROP INDEX [IX_ChatParticipants_UserId1] ON [ChatParticipants];
            ");
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ChatMessages_ChatRoomId1')
                    DROP INDEX [IX_ChatMessages_ChatRoomId1] ON [ChatMessages];
            ");
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ChatMessages_UserId')
                    DROP INDEX [IX_ChatMessages_UserId] ON [ChatMessages];
            ");

            // Conditional drops for Columns
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.columns WHERE name = 'SemesterId1' AND object_id = OBJECT_ID('TermKeywords'))
                    ALTER TABLE [TermKeywords] DROP COLUMN [SemesterId1];
            ");
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.columns WHERE name = 'UserId1' AND object_id = OBJECT_ID('Submissions'))
                    ALTER TABLE [Submissions] DROP COLUMN [UserId1];
            ");
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.columns WHERE name = 'UserId1' AND object_id = OBJECT_ID('RedeemRequests'))
                    ALTER TABLE [RedeemRequests] DROP COLUMN [UserId1];
            ");
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.columns WHERE name = 'SemesterId1' AND object_id = OBJECT_ID('Projects'))
                    ALTER TABLE [Projects] DROP COLUMN [SemesterId1];
            ");
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.columns WHERE name = 'MilestoneId1' AND object_id = OBJECT_ID('PerformanceScores'))
                    ALTER TABLE [PerformanceScores] DROP COLUMN [MilestoneId1];
            ");
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.columns WHERE name = 'ProjectId1' AND object_id = OBJECT_ID('PerformanceScores'))
                    ALTER TABLE [PerformanceScores] DROP COLUMN [ProjectId1];
            ");
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.columns WHERE name = 'TaskId1' AND object_id = OBJECT_ID('PerformanceScores'))
                    ALTER TABLE [PerformanceScores] DROP COLUMN [TaskId1];
            ");
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.columns WHERE name = 'UserId1' AND object_id = OBJECT_ID('PerformanceScores'))
                    ALTER TABLE [PerformanceScores] DROP COLUMN [UserId1];
            ");
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.columns WHERE name = 'NotificationTemplateId1' AND object_id = OBJECT_ID('Notifications'))
                    ALTER TABLE [Notifications] DROP COLUMN [NotificationTemplateId1];
            ");
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.columns WHERE name = 'UserId1' AND object_id = OBJECT_ID('Notifications'))
                    ALTER TABLE [Notifications] DROP COLUMN [UserId1];
            ");
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.columns WHERE name = 'UserId1' AND object_id = OBJECT_ID('ChatParticipants'))
                    ALTER TABLE [ChatParticipants] DROP COLUMN [UserId1];
            ");
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.columns WHERE name = 'ChatRoomId1' AND object_id = OBJECT_ID('ChatMessages'))
                    ALTER TABLE [ChatMessages] DROP COLUMN [ChatRoomId1];
            ");
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.columns WHERE name = 'UserId' AND object_id = OBJECT_ID('ChatMessages'))
                    ALTER TABLE [ChatMessages] DROP COLUMN [UserId];
            ");

            // Safe AlterColumn (no-op if already matching)
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

            // Adds (EF will skip if already exist)
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
            // Conditional drops for new FKs
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ChatRooms_Users_UserId')
                    ALTER TABLE [ChatRooms] DROP CONSTRAINT [FK_ChatRooms_Users_UserId];
            ");
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Tasks_Users_AssigneeId')
                    ALTER TABLE [Tasks] DROP CONSTRAINT [FK_Tasks_Users_AssigneeId];
            ");
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_UserAchievements_Achievements_AchievementId')
                    ALTER TABLE [UserAchievements] DROP CONSTRAINT [FK_UserAchievements_Achievements_AchievementId];
            ");
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_UserAchievements_Users_UserId')
                    ALTER TABLE [UserAchievements] DROP CONSTRAINT [FK_UserAchievements_Users_UserId];
            ");

            // Conditional adds for columns (in reverse order)
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE name = 'SemesterId1' AND object_id = OBJECT_ID('TermKeywords'))
                    ALTER TABLE [TermKeywords] ADD [SemesterId1] int NULL;
            ");
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE name = 'UserId1' AND object_id = OBJECT_ID('Submissions'))
                    ALTER TABLE [Submissions] ADD [UserId1] int NULL;
            ");
            migrationBuilder.AlterColumn<DateTime>(
                name: "ProcessedAt",
                table: "RedeemRequests",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE name = 'UserId1' AND object_id = OBJECT_ID('RedeemRequests'))
                    ALTER TABLE [RedeemRequests] ADD [UserId1] int NULL;
            ");
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE name = 'SemesterId1' AND object_id = OBJECT_ID('Projects'))
                    ALTER TABLE [Projects] ADD [SemesterId1] int NULL;
            ");
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE name = 'MilestoneId1' AND object_id = OBJECT_ID('PerformanceScores'))
                    ALTER TABLE [PerformanceScores] ADD [MilestoneId1] int NULL;
            ");
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE name = 'ProjectId1' AND object_id = OBJECT_ID('PerformanceScores'))
                    ALTER TABLE [PerformanceScores] ADD [ProjectId1] int NULL;
            ");
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE name = 'TaskId1' AND object_id = OBJECT_ID('PerformanceScores'))
                    ALTER TABLE [PerformanceScores] ADD [TaskId1] int NULL;
            ");
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE name = 'UserId1' AND object_id = OBJECT_ID('PerformanceScores'))
                    ALTER TABLE [PerformanceScores] ADD [UserId1] int NULL;
            ");
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE name = 'NotificationTemplateId1' AND object_id = OBJECT_ID('Notifications'))
                    ALTER TABLE [Notifications] ADD [NotificationTemplateId1] int NULL;
            ");
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE name = 'UserId1' AND object_id = OBJECT_ID('Notifications'))
                    ALTER TABLE [Notifications] ADD [UserId1] int NULL;
            ");
            migrationBuilder.AlterColumn<int>(
                name: "ProjectId",
                table: "ChatRooms",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE name = 'UserId1' AND object_id = OBJECT_ID('ChatParticipants'))
                    ALTER TABLE [ChatParticipants] ADD [UserId1] int NULL;
            ");
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE name = 'ChatRoomId1' AND object_id = OBJECT_ID('ChatMessages'))
                    ALTER TABLE [ChatMessages] ADD [ChatRoomId1] int NULL;
            ");
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE name = 'UserId' AND object_id = OBJECT_ID('ChatMessages'))
                    ALTER TABLE [ChatMessages] ADD [UserId] int NULL;
            ");

            // Conditional creates for indexes
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_TermKeywords_SemesterId1')
                    CREATE INDEX [IX_TermKeywords_SemesterId1] ON [TermKeywords] ([SemesterId1]);
            ");
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Submissions_UserId1')
                    CREATE INDEX [IX_Submissions_UserId1] ON [Submissions] ([UserId1]);
            ");
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_RedeemRequests_UserId1')
                    CREATE INDEX [IX_RedeemRequests_UserId1] ON [RedeemRequests] ([UserId1]);
            ");
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Projects_SemesterId1')
                    CREATE INDEX [IX_Projects_SemesterId1] ON [Projects] ([SemesterId1]);
            ");
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_PerformanceScores_MilestoneId1')
                    CREATE INDEX [IX_PerformanceScores_MilestoneId1] ON [PerformanceScores] ([MilestoneId1]);
            ");
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_PerformanceScores_ProjectId1')
                    CREATE INDEX [IX_PerformanceScores_ProjectId1] ON [PerformanceScores] ([ProjectId1]);
            ");
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_PerformanceScores_TaskId1')
                    CREATE INDEX [IX_PerformanceScores_TaskId1] ON [PerformanceScores] ([TaskId1]);
            ");
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_PerformanceScores_UserId1')
                    CREATE INDEX [IX_PerformanceScores_UserId1] ON [PerformanceScores] ([UserId1]);
            ");
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Notifications_NotificationTemplateId1')
                    CREATE INDEX [IX_Notifications_NotificationTemplateId1] ON [Notifications] ([NotificationTemplateId1]);
            ");
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Notifications_UserId1')
                    CREATE INDEX [IX_Notifications_UserId1] ON [Notifications] ([UserId1]);
            ");
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ChatParticipants_UserId1')
                    CREATE INDEX [IX_ChatParticipants_UserId1] ON [ChatParticipants] ([UserId1]);
            ");
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ChatMessages_ChatRoomId1')
                    CREATE INDEX [IX_ChatMessages_ChatRoomId1] ON [ChatMessages] ([ChatRoomId1]);
            ");
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ChatMessages_UserId')
                    CREATE INDEX [IX_ChatMessages_UserId] ON [ChatMessages] ([UserId]);
            ");

            // Adds for old FKs (conditional to avoid duplicates)
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ChatMessages_ChatRooms_ChatRoomId1')
                    ALTER TABLE [ChatMessages] ADD CONSTRAINT [FK_ChatMessages_ChatRooms_ChatRoomId1] FOREIGN KEY ([ChatRoomId1]) REFERENCES [ChatRooms] ([Id]);
            ");
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ChatMessages_Users_UserId')
                    ALTER TABLE [ChatMessages] ADD CONSTRAINT [FK_ChatMessages_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]);
            ");
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ChatParticipants_Users_UserId1')
                    ALTER TABLE [ChatParticipants] ADD CONSTRAINT [FK_ChatParticipants_Users_UserId1] FOREIGN KEY ([UserId1]) REFERENCES [Users] ([Id]);
            ");
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ChatRooms_Users_UserId')
                    ALTER TABLE [ChatRooms] ADD CONSTRAINT [FK_ChatRooms_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE;
            ");
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Notifications_NotificationTemplates_NotificationTemplateId1')
                    ALTER TABLE [Notifications] ADD CONSTRAINT [FK_Notifications_NotificationTemplates_NotificationTemplateId1] FOREIGN KEY ([NotificationTemplateId1]) REFERENCES [NotificationTemplates] ([Id]);
            ");
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Notifications_Users_UserId1')
                    ALTER TABLE [Notifications] ADD CONSTRAINT [FK_Notifications_Users_UserId1] FOREIGN KEY ([UserId1]) REFERENCES [Users] ([Id]);
            ");
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_PerformanceScores_Milestones_MilestoneId1')
                    ALTER TABLE [PerformanceScores] ADD CONSTRAINT [FK_PerformanceScores_Milestones_MilestoneId1] FOREIGN KEY ([MilestoneId1]) REFERENCES [Milestones] ([Id]);
            ");
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_PerformanceScores_Projects_ProjectId1')
                    ALTER TABLE [PerformanceScores] ADD CONSTRAINT [FK_PerformanceScores_Projects_ProjectId1] FOREIGN KEY ([ProjectId1]) REFERENCES [Projects] ([Id]);
            ");
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_PerformanceScores_Tasks_TaskId1')
                    ALTER TABLE [PerformanceScores] ADD CONSTRAINT [FK_PerformanceScores_Tasks_TaskId1] FOREIGN KEY ([TaskId1]) REFERENCES [Tasks] ([Id]);
            ");
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_PerformanceScores_Users_UserId1')
                    ALTER TABLE [PerformanceScores] ADD CONSTRAINT [FK_PerformanceScores_Users_UserId1] FOREIGN KEY ([UserId1]) REFERENCES [Users] ([Id]);
            ");
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Projects_Semesters_SemesterId1')
                    ALTER TABLE [Projects] ADD CONSTRAINT [FK_Projects_Semesters_SemesterId1] FOREIGN KEY ([SemesterId1]) REFERENCES [Semesters] ([Id]);
            ");
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_RedeemRequests_Users_UserId1')
                    ALTER TABLE [RedeemRequests] ADD CONSTRAINT [FK_RedeemRequests_Users_UserId1] FOREIGN KEY ([UserId1]) REFERENCES [Users] ([Id]);
            ");
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Submissions_Users_UserId1')
                    ALTER TABLE [Submissions] ADD CONSTRAINT [FK_Submissions_Users_UserId1] FOREIGN KEY ([UserId1]) REFERENCES [Users] ([Id]);
            ");
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Tasks_Users_AssigneeId')
                    ALTER TABLE [Tasks] ADD CONSTRAINT [FK_Tasks_Users_AssigneeId] FOREIGN KEY ([AssigneeId]) REFERENCES [Users] ([Id]);
            ");
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_TermKeywords_Semesters_SemesterId1')
                    ALTER TABLE [TermKeywords] ADD CONSTRAINT [FK_TermKeywords_Semesters_SemesterId1] FOREIGN KEY ([SemesterId1]) REFERENCES [Semesters] ([Id]);
            ");
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_UserAchievements_Achievements_AchievementId')
                    ALTER TABLE [UserAchievements] ADD CONSTRAINT [FK_UserAchievements_Achievements_AchievementId] FOREIGN KEY ([AchievementId]) REFERENCES [Achievements] ([Id]) ON DELETE CASCADE;
            ");
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_UserAchievements_Users_UserId')
                    ALTER TABLE [UserAchievements] ADD CONSTRAINT [FK_UserAchievements_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE;
            ");
        }
    }
}
using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FGS_BE.Repo.Migrations
{
    /// <inheritdoc />
    public partial class AddVerificationTokenToUserAndAddNewStatusToRedeemRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "VerificationToken",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CollectedAt",
                table: "RedeemRequests",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VerificationToken",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CollectedAt",
                table: "RedeemRequests");
        }
    }
}

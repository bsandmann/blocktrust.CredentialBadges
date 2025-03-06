using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Blocktrust.CredentialBadges.Web.Migrations
{
    /// <inheritdoc />
    public partial class validUntilNotMantatory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "ValidUntil",
                table: "VerifiedCredentials",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "ValidUntil",
                table: "VerifiedCredentials",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);
        }
    }
}

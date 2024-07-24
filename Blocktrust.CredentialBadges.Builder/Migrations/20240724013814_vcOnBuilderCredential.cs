using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Blocktrust.CredentialBadges.Builder.Migrations
{
    /// <inheritdoc />
    public partial class vcOnBuilderCredential : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("3716c332-9b21-4ccf-a4b6-6611614da2c8"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("a1521a0a-3dc1-4c03-a4fa-1e1fe4aa7fcd"));

            migrationBuilder.AddColumn<string>(
                name: "VerifiableCredential",
                table: "BuilderCredentials",
                type: "text",
                nullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("2d3278e5-0fee-461a-81a2-b7e4e4df81f8"), null, "adminRole", "ADMINROLE" },
                    { new Guid("9bf6af0e-26d5-4bd0-9646-5e33ff7d85ea"), null, "nonAdminRole", "NONADMINROLE" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("2d3278e5-0fee-461a-81a2-b7e4e4df81f8"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("9bf6af0e-26d5-4bd0-9646-5e33ff7d85ea"));

            migrationBuilder.DropColumn(
                name: "VerifiableCredential",
                table: "BuilderCredentials");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("3716c332-9b21-4ccf-a4b6-6611614da2c8"), null, "nonAdminRole", "NONADMINROLE" },
                    { new Guid("a1521a0a-3dc1-4c03-a4fa-1e1fe4aa7fcd"), null, "adminRole", "ADMINROLE" }
                });
        }
    }
}

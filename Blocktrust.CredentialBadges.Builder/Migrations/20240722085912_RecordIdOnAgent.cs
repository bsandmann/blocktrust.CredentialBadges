using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Blocktrust.CredentialBadges.Builder.Migrations
{
    /// <inheritdoc />
    public partial class RecordIdOnAgent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("114edd3f-df2a-46ab-a76c-f32806032957"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("7bcaa74e-9590-4d2f-ac93-ea5be0a8b863"));

            migrationBuilder.AddColumn<Guid>(
                name: "RecordIdOnAgent",
                table: "BuilderCredentials",
                type: "uuid",
                nullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("3716c332-9b21-4ccf-a4b6-6611614da2c8"), null, "nonAdminRole", "NONADMINROLE" },
                    { new Guid("a1521a0a-3dc1-4c03-a4fa-1e1fe4aa7fcd"), null, "adminRole", "ADMINROLE" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("3716c332-9b21-4ccf-a4b6-6611614da2c8"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("a1521a0a-3dc1-4c03-a4fa-1e1fe4aa7fcd"));

            migrationBuilder.DropColumn(
                name: "RecordIdOnAgent",
                table: "BuilderCredentials");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("114edd3f-df2a-46ab-a76c-f32806032957"), null, "nonAdminRole", "NONADMINROLE" },
                    { new Guid("7bcaa74e-9590-4d2f-ac93-ea5be0a8b863"), null, "adminRole", "ADMINROLE" }
                });
        }
    }
}

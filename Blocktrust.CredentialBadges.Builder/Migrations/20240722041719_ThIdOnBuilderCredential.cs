using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Blocktrust.CredentialBadges.Builder.Migrations
{
    /// <inheritdoc />
    public partial class ThIdOnBuilderCredential : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("820c077c-69c6-4489-912c-c9f3e2737864"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("a0fcc7a6-3a1f-43f2-b07b-7f01a9b4205a"));

            migrationBuilder.AddColumn<Guid>(
                name: "ThId",
                table: "BuilderCredentials",
                type: "uuid",
                nullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("114edd3f-df2a-46ab-a76c-f32806032957"), null, "nonAdminRole", "NONADMINROLE" },
                    { new Guid("7bcaa74e-9590-4d2f-ac93-ea5be0a8b863"), null, "adminRole", "ADMINROLE" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("114edd3f-df2a-46ab-a76c-f32806032957"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("7bcaa74e-9590-4d2f-ac93-ea5be0a8b863"));

            migrationBuilder.DropColumn(
                name: "ThId",
                table: "BuilderCredentials");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("820c077c-69c6-4489-912c-c9f3e2737864"), null, "adminRole", "ADMINROLE" },
                    { new Guid("a0fcc7a6-3a1f-43f2-b07b-7f01a9b4205a"), null, "nonAdminRole", "NONADMINROLE" }
                });
        }
    }
}

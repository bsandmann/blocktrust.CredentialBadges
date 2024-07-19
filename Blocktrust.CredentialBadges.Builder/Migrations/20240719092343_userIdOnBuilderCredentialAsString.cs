using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Blocktrust.CredentialBadges.Builder.Migrations
{
    /// <inheritdoc />
    public partial class userIdOnBuilderCredentialAsString : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("97a5aa8b-a2c9-4806-9787-7caaf76b51a9"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("ea7f7c07-c7c3-4b39-96b7-e1afd5ecb58c"));

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("820c077c-69c6-4489-912c-c9f3e2737864"), null, "adminRole", "ADMINROLE" },
                    { new Guid("a0fcc7a6-3a1f-43f2-b07b-7f01a9b4205a"), null, "nonAdminRole", "NONADMINROLE" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("820c077c-69c6-4489-912c-c9f3e2737864"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("a0fcc7a6-3a1f-43f2-b07b-7f01a9b4205a"));

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("97a5aa8b-a2c9-4806-9787-7caaf76b51a9"), null, "nonAdminRole", "NONADMINROLE" },
                    { new Guid("ea7f7c07-c7c3-4b39-96b7-e1afd5ecb58c"), null, "adminRole", "ADMINROLE" }
                });
        }
    }
}

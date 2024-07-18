using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Blocktrust.CredentialBadges.Builder.Migrations
{
    /// <inheritdoc />
    public partial class buildercredentialentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("c0481c96-2c99-4ccc-9039-1165080cb702"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("fb01458f-cd7b-43c4-8646-03e9a3476934"));

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("4384eb41-d40a-46ee-899a-e2e9967cd8b3"), null, "nonAdminRole", "NONADMINROLE" },
                    { new Guid("94da5d25-1c2d-4ca8-9073-91512f96aaae"), null, "adminRole", "ADMINROLE" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("4384eb41-d40a-46ee-899a-e2e9967cd8b3"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("94da5d25-1c2d-4ca8-9073-91512f96aaae"));

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("c0481c96-2c99-4ccc-9039-1165080cb702"), null, "adminRole", "ADMINROLE" },
                    { new Guid("fb01458f-cd7b-43c4-8646-03e9a3476934"), null, "nonAdminRole", "NONADMINROLE" }
                });
        }
    }
}

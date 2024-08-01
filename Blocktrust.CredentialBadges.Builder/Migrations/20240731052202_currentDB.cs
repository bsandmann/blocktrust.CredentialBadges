using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Blocktrust.CredentialBadges.Builder.Migrations
{
    /// <inheritdoc />
    public partial class currentDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("2d3278e5-0fee-461a-81a2-b7e4e4df81f8"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("9bf6af0e-26d5-4bd0-9646-5e33ff7d85ea"));

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("c131feb1-f7b1-404e-b31b-a9eb6094f0d9"), null, "adminRole", "ADMINROLE" },
                    { new Guid("d89e9303-e49c-4083-af96-9dbc6833dfa4"), null, "nonAdminRole", "NONADMINROLE" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("c131feb1-f7b1-404e-b31b-a9eb6094f0d9"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("d89e9303-e49c-4083-af96-9dbc6833dfa4"));

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("2d3278e5-0fee-461a-81a2-b7e4e4df81f8"), null, "adminRole", "ADMINROLE" },
                    { new Guid("9bf6af0e-26d5-4bd0-9646-5e33ff7d85ea"), null, "nonAdminRole", "NONADMINROLE" }
                });
        }
    }
}

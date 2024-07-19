using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Blocktrust.CredentialBadges.Builder.Migrations
{
    /// <inheritdoc />
    public partial class userIdOnBuilderCredential : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("067e7bb1-67a3-40ed-a18f-b15ac8a66a1d"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("5a38e9d4-8958-4c0c-8968-f0d322ebe300"));

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "BuilderCredentials",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("97a5aa8b-a2c9-4806-9787-7caaf76b51a9"), null, "nonAdminRole", "NONADMINROLE" },
                    { new Guid("ea7f7c07-c7c3-4b39-96b7-e1afd5ecb58c"), null, "adminRole", "ADMINROLE" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("97a5aa8b-a2c9-4806-9787-7caaf76b51a9"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("ea7f7c07-c7c3-4b39-96b7-e1afd5ecb58c"));

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "BuilderCredentials");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("067e7bb1-67a3-40ed-a18f-b15ac8a66a1d"), null, "adminRole", "ADMINROLE" },
                    { new Guid("5a38e9d4-8958-4c0c-8968-f0d322ebe300"), null, "nonAdminRole", "NONADMINROLE" }
                });
        }
    }
}

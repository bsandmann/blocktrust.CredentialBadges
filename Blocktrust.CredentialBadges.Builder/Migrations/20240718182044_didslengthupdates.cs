using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Blocktrust.CredentialBadges.Builder.Migrations
{
    /// <inheritdoc />
    public partial class didslengthupdates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("cbe05ad7-f17d-4f9e-be55-fae983992f78"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("ce181686-0854-48c2-b202-de7537b6bfb2"));

            migrationBuilder.AlterColumn<string>(
                name: "SubjectDid",
                table: "BuilderCredentials",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Label",
                table: "BuilderCredentials",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "IssuerDid",
                table: "BuilderCredentials",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("067e7bb1-67a3-40ed-a18f-b15ac8a66a1d"), null, "adminRole", "ADMINROLE" },
                    { new Guid("5a38e9d4-8958-4c0c-8968-f0d322ebe300"), null, "nonAdminRole", "NONADMINROLE" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("067e7bb1-67a3-40ed-a18f-b15ac8a66a1d"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("5a38e9d4-8958-4c0c-8968-f0d322ebe300"));

            migrationBuilder.AlterColumn<string>(
                name: "SubjectDid",
                table: "BuilderCredentials",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<string>(
                name: "Label",
                table: "BuilderCredentials",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256);

            migrationBuilder.AlterColumn<string>(
                name: "IssuerDid",
                table: "BuilderCredentials",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("cbe05ad7-f17d-4f9e-be55-fae983992f78"), null, "adminRole", "ADMINROLE" },
                    { new Guid("ce181686-0854-48c2-b202-de7537b6bfb2"), null, "nonAdminRole", "NONADMINROLE" }
                });
        }
    }
}

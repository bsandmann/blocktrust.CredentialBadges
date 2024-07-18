using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Blocktrust.CredentialBadges.Builder.Migrations
{
    /// <inheritdoc />
    public partial class buildercredentialentityupdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("4384eb41-d40a-46ee-899a-e2e9967cd8b3"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("94da5d25-1c2d-4ca8-9073-91512f96aaae"));

            migrationBuilder.CreateTable(
                name: "BuilderCredentials",
                columns: table => new
                {
                    CredentialId = table.Column<Guid>(type: "uuid", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Label = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    SubjectDid = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    IssuerDid = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    IssuerConnectionId = table.Column<Guid>(type: "uuid", nullable: false),
                    SubjectConnectionId = table.Column<Guid>(type: "uuid", nullable: false),
                    CredentialSubject = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BuilderCredentials", x => x.CredentialId);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("cbe05ad7-f17d-4f9e-be55-fae983992f78"), null, "adminRole", "ADMINROLE" },
                    { new Guid("ce181686-0854-48c2-b202-de7537b6bfb2"), null, "nonAdminRole", "NONADMINROLE" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BuilderCredentials");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("cbe05ad7-f17d-4f9e-be55-fae983992f78"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("ce181686-0854-48c2-b202-de7537b6bfb2"));

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("4384eb41-d40a-46ee-899a-e2e9967cd8b3"), null, "nonAdminRole", "NONADMINROLE" },
                    { new Guid("94da5d25-1c2d-4ca8-9073-91512f96aaae"), null, "adminRole", "ADMINROLE" }
                });
        }
    }
}

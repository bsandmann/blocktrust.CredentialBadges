using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Blocktrust.CredentialBadges.Web.Migrations
{
    /// <inheritdoc />
    public partial class Madedomainandimagenullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Image",
                table: "VerifiedCredentials",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Domain",
                table: "VerifiedCredentials",
                type: "character varying(253)",
                maxLength: 253,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(253)",
                oldMaxLength: 253);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Image",
                table: "VerifiedCredentials",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Domain",
                table: "VerifiedCredentials",
                type: "character varying(253)",
                maxLength: 253,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(253)",
                oldMaxLength: 253,
                oldNullable: true);
        }
    }
}

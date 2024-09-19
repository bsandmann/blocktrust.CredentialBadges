using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Blocktrust.CredentialBadges.Web.Migrations
{
    /// <inheritdoc />
    public partial class addedrawjwtfieldinthecrtedential : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RawJwt",
                table: "VerifiedCredentials",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RawJwt",
                table: "VerifiedCredentials");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Blocktrust.CredentialBadges.Web.Migrations
{
    /// <inheritdoc />
    public partial class credentialClaims : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Claims",
                table: "VerifiedCredentials",
                type: "text",
                maxLength: 5000,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Claims",
                table: "VerifiedCredentials");
        }
    }
}

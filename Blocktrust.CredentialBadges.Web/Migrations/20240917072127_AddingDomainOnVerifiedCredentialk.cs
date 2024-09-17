using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Blocktrust.CredentialBadges.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddingDomainOnVerifiedCredentialk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Domain",
                table: "VerifiedCredentials",
                type: "character varying(253)",
                maxLength: 253,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Domain",
                table: "VerifiedCredentials");
        }
    }
}

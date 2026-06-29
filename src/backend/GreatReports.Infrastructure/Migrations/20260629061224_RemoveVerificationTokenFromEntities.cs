using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GreatReports.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveVerificationTokenFromEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VerificationToken",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "EmailConfirmed",
                table: "ClientContacts");

            migrationBuilder.DropColumn(
                name: "VerificationToken",
                table: "ClientContacts");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "VerificationToken",
                table: "Users",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EmailConfirmed",
                table: "ClientContacts",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "VerificationToken",
                table: "ClientContacts",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true);
        }
    }
}

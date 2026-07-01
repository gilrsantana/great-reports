using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GreatReports.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ManagerOfCompanyProviderEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // MySQL requires DROP FOREIGN KEY instead of DROP CONSTRAINT.
            // The .NET MySQL connector interprets @variables as command parameters,
            // so we use a temporary stored procedure to work around this.
            migrationBuilder.Sql(
                "DROP PROCEDURE IF EXISTS `_EF_DropFkIfExists`;");

            migrationBuilder.Sql(@"
                CREATE PROCEDURE `_EF_DropFkIfExists`()
                BEGIN
                    DECLARE fk_count INT;
                    SELECT COUNT(*) INTO fk_count 
                    FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS 
                    WHERE CONSTRAINT_NAME = 'FK_ProviderCompanies_Users_ManagerId' 
                      AND TABLE_NAME = 'ProviderCompanies' 
                      AND CONSTRAINT_TYPE = 'FOREIGN KEY';
                    IF fk_count > 0 THEN
                        ALTER TABLE `ProviderCompanies` DROP FOREIGN KEY `FK_ProviderCompanies_Users_ManagerId`;
                    END IF;

                    SELECT COUNT(*) INTO fk_count 
                    FROM INFORMATION_SCHEMA.STATISTICS 
                    WHERE INDEX_NAME = 'IX_ProviderCompanies_ManagerId' 
                      AND TABLE_NAME = 'ProviderCompanies';
                    IF fk_count > 0 THEN
                        ALTER TABLE `ProviderCompanies` DROP INDEX `IX_ProviderCompanies_ManagerId`;
                    END IF;
                END;
            ");

            migrationBuilder.Sql("CALL `_EF_DropFkIfExists`();");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS `_EF_DropFkIfExists`;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_ProviderCompanies_ManagerId",
                table: "ProviderCompanies",
                column: "ManagerId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ProviderCompanies_Users_ManagerId",
                table: "ProviderCompanies",
                column: "ManagerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}


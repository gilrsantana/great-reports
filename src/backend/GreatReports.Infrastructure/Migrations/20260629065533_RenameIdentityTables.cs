using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GreatReports.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameIdentityTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop foreign keys using MySQL-compatible ALTER TABLE DROP FOREIGN KEY syntax
            migrationBuilder.Sql("ALTER TABLE `AccountClaims` DROP FOREIGN KEY `FK_AccountClaims_Accounts_UserId`;");
            migrationBuilder.Sql("ALTER TABLE `AccountLogins` DROP FOREIGN KEY `FK_AccountLogins_Accounts_UserId`;");
            migrationBuilder.Sql("ALTER TABLE `AccountRoles` DROP FOREIGN KEY `FK_AccountRoles_Accounts_UserId`;");
            migrationBuilder.Sql("ALTER TABLE `AccountRoles` DROP FOREIGN KEY `FK_AccountRoles_Roles_RoleId`;");
            migrationBuilder.Sql("ALTER TABLE `AccountTokens` DROP FOREIGN KEY `FK_AccountTokens_Accounts_UserId`;");
            migrationBuilder.Sql("ALTER TABLE `RoleClaims` DROP FOREIGN KEY `FK_RoleClaims_Roles_RoleId`;");

            // Rename tables
            migrationBuilder.RenameTable(name: "Roles", newName: "IdentityRoles");
            migrationBuilder.RenameTable(name: "RoleClaims", newName: "IdentityRoleClaims");
            migrationBuilder.RenameTable(name: "AccountTokens", newName: "IdentityUserTokens");
            migrationBuilder.RenameTable(name: "Accounts", newName: "IdentityUsers");
            migrationBuilder.RenameTable(name: "AccountRoles", newName: "IdentityUserRoles");
            migrationBuilder.RenameTable(name: "AccountLogins", newName: "IdentityUserLogins");
            migrationBuilder.RenameTable(name: "AccountClaims", newName: "IdentityUserClaims");

            // Rename indexes
            migrationBuilder.RenameIndex(
                name: "IX_RoleClaims_RoleId",
                table: "IdentityRoleClaims",
                newName: "IX_IdentityRoleClaims_RoleId");

            migrationBuilder.RenameIndex(
                name: "IX_AccountRoles_RoleId",
                table: "IdentityUserRoles",
                newName: "IX_IdentityUserRoles_RoleId");

            migrationBuilder.RenameIndex(
                name: "IX_AccountLogins_UserId",
                table: "IdentityUserLogins",
                newName: "IX_IdentityUserLogins_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_AccountClaims_UserId",
                table: "IdentityUserClaims",
                newName: "IX_IdentityUserClaims_UserId");

            // Add foreign keys pointing to the renamed tables
            migrationBuilder.AddForeignKey(
                name: "FK_IdentityRoleClaims_IdentityRoles_RoleId",
                table: "IdentityRoleClaims",
                column: "RoleId",
                principalTable: "IdentityRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_IdentityUserClaims_IdentityUsers_UserId",
                table: "IdentityUserClaims",
                column: "UserId",
                principalTable: "IdentityUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_IdentityUserLogins_IdentityUsers_UserId",
                table: "IdentityUserLogins",
                column: "UserId",
                principalTable: "IdentityUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_IdentityUserRoles_IdentityRoles_RoleId",
                table: "IdentityUserRoles",
                column: "RoleId",
                principalTable: "IdentityRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_IdentityUserRoles_IdentityUsers_UserId",
                table: "IdentityUserRoles",
                column: "UserId",
                principalTable: "IdentityUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_IdentityUserTokens_IdentityUsers_UserId",
                table: "IdentityUserTokens",
                column: "UserId",
                principalTable: "IdentityUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop foreign keys using MySQL-compatible ALTER TABLE DROP FOREIGN KEY syntax
            migrationBuilder.Sql("ALTER TABLE `IdentityRoleClaims` DROP FOREIGN KEY `FK_IdentityRoleClaims_IdentityRoles_RoleId`;");
            migrationBuilder.Sql("ALTER TABLE `IdentityUserClaims` DROP FOREIGN KEY `FK_IdentityUserClaims_IdentityUsers_UserId`;");
            migrationBuilder.Sql("ALTER TABLE `IdentityUserLogins` DROP FOREIGN KEY `FK_IdentityUserLogins_IdentityUsers_UserId`;");
            migrationBuilder.Sql("ALTER TABLE `IdentityUserRoles` DROP FOREIGN KEY `FK_IdentityUserRoles_IdentityRoles_RoleId`;");
            migrationBuilder.Sql("ALTER TABLE `IdentityUserRoles` DROP FOREIGN KEY `FK_IdentityUserRoles_IdentityUsers_UserId`;");
            migrationBuilder.Sql("ALTER TABLE `IdentityUserTokens` DROP FOREIGN KEY `FK_IdentityUserTokens_IdentityUsers_UserId`;");

            // Rename tables back
            migrationBuilder.RenameTable(name: "IdentityUserTokens", newName: "AccountTokens");
            migrationBuilder.RenameTable(name: "IdentityUsers", newName: "Accounts");
            migrationBuilder.RenameTable(name: "IdentityUserRoles", newName: "AccountRoles");
            migrationBuilder.RenameTable(name: "IdentityUserLogins", newName: "AccountLogins");
            migrationBuilder.RenameTable(name: "IdentityUserClaims", newName: "AccountClaims");
            migrationBuilder.RenameTable(name: "IdentityRoles", newName: "Roles");
            migrationBuilder.RenameTable(name: "IdentityRoleClaims", newName: "RoleClaims");

            // Rename indexes back
            migrationBuilder.RenameIndex(
                name: "IX_IdentityUserRoles_RoleId",
                table: "AccountRoles",
                newName: "IX_AccountRoles_RoleId");

            migrationBuilder.RenameIndex(
                name: "IX_IdentityUserLogins_UserId",
                table: "AccountLogins",
                newName: "IX_AccountLogins_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_IdentityUserClaims_UserId",
                table: "AccountClaims",
                newName: "IX_AccountClaims_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_IdentityRoleClaims_RoleId",
                table: "RoleClaims",
                newName: "IX_RoleClaims_RoleId");

            // Add foreign keys back pointing to original tables
            migrationBuilder.AddForeignKey(
                name: "FK_AccountClaims_Accounts_UserId",
                table: "AccountClaims",
                column: "UserId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AccountLogins_Accounts_UserId",
                table: "AccountLogins",
                column: "UserId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AccountRoles_Accounts_UserId",
                table: "AccountRoles",
                column: "UserId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AccountRoles_Roles_RoleId",
                table: "AccountRoles",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AccountTokens_Accounts_UserId",
                table: "AccountTokens",
                column: "UserId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RoleClaims_Roles_RoleId",
                table: "RoleClaims",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

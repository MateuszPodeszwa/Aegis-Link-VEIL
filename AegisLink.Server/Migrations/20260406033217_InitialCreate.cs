using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AegisLink.Server.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserKeys",
                columns: table => new
                {
                    AegisId = table.Column<string>(type: "TEXT", maxLength: 8, nullable: false),
                    PublicKey = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserKeys", x => x.AegisId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserKeys_PublicKey",
                table: "UserKeys",
                column: "PublicKey",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserKeys");
        }
    }
}

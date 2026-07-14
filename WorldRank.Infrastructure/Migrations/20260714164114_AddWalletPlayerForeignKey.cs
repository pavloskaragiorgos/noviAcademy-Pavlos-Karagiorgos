using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorldRank.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddWalletPlayerForeignKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Wallets_PlayerId",
                table: "Wallets",
                column: "PlayerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Wallets_Players_PlayerId",
                table: "Wallets",
                column: "PlayerId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Wallets_Players_PlayerId",
                table: "Wallets");

            migrationBuilder.DropIndex(
                name: "IX_Wallets_PlayerId",
                table: "Wallets");
        }
    }
}

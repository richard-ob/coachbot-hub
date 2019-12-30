using Microsoft.EntityFrameworkCore.Migrations;

namespace CoachBot.Domain.Migrations
{
    public partial class Initial711814941 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PlayerTeamPositions_PositionId",
                table: "PlayerTeamPositions");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerTeamPositions_PositionId_TeamId",
                table: "PlayerTeamPositions",
                columns: new[] { "PositionId", "TeamId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PlayerTeamPositions_PositionId_TeamId",
                table: "PlayerTeamPositions");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerTeamPositions_PositionId",
                table: "PlayerTeamPositions",
                column: "PositionId");
        }
    }
}

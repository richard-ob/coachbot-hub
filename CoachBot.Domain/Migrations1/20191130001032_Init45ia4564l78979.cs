using Microsoft.EntityFrameworkCore.Migrations;

namespace CoachBot.Domain.Migrations
{
    public partial class Init45ia4564l78979 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MatchStatistics_MatchId",
                table: "MatchStatistics");

            migrationBuilder.AddColumn<bool>(
                name: "DuplicityProtection",
                table: "Channels",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_MatchStatistics_MatchId",
                table: "MatchStatistics",
                column: "MatchId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MatchStatistics_MatchId",
                table: "MatchStatistics");

            migrationBuilder.DropColumn(
                name: "DuplicityProtection",
                table: "Channels");

            migrationBuilder.CreateIndex(
                name: "IX_MatchStatistics_MatchId",
                table: "MatchStatistics",
                column: "MatchId");
        }
    }
}

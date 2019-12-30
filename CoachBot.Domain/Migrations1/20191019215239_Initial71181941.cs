using Microsoft.EntityFrameworkCore.Migrations;

namespace CoachBot.Domain.Migrations
{
    public partial class Initial71181941 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "Servers",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "RegionName",
                table: "Regions",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Servers_Address",
                table: "Servers",
                column: "Address",
                unique: true,
                filter: "[Address] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Regions_RegionName",
                table: "Regions",
                column: "RegionName",
                unique: true,
                filter: "[RegionName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Channels_DiscordChannelId",
                table: "Channels",
                column: "DiscordChannelId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Servers_Address",
                table: "Servers");

            migrationBuilder.DropIndex(
                name: "IX_Regions_RegionName",
                table: "Regions");

            migrationBuilder.DropIndex(
                name: "IX_Channels_DiscordChannelId",
                table: "Channels");

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "Servers",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "RegionName",
                table: "Regions",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}

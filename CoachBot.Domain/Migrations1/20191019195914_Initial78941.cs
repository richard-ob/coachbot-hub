using Microsoft.EntityFrameworkCore.Migrations;

namespace CoachBot.Domain.Migrations
{
    public partial class Initial78941 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Channels_Guild_GuildId",
                table: "Channels");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Guild",
                table: "Guild");

            migrationBuilder.RenameTable(
                name: "Guild",
                newName: "Guilds");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Guilds",
                table: "Guilds",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Channels_Guilds_GuildId",
                table: "Channels",
                column: "GuildId",
                principalTable: "Guilds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Channels_Guilds_GuildId",
                table: "Channels");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Guilds",
                table: "Guilds");

            migrationBuilder.RenameTable(
                name: "Guilds",
                newName: "Guild");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Guild",
                table: "Guild",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Channels_Guild_GuildId",
                table: "Channels",
                column: "GuildId",
                principalTable: "Guild",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

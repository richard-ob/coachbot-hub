using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CoachBot.Domain.Migrations
{
    public partial class Initial78979 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SubstitutionRequests",
                columns: table => new
                {
                    Token = table.Column<string>(nullable: false),
                    ChannelId = table.Column<int>(nullable: false),
                    ServerId = table.Column<int>(nullable: false),
                    DiscordMessageId = table.Column<decimal>(nullable: false),
                    AcceptedById = table.Column<int>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    AcceptedDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubstitutionRequests", x => x.Token);
                    table.ForeignKey(
                        name: "FK_SubstitutionRequests_Players_AcceptedById",
                        column: x => x.AcceptedById,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SubstitutionRequests_Channels_ChannelId",
                        column: x => x.ChannelId,
                        principalTable: "Channels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubstitutionRequests_Servers_ServerId",
                        column: x => x.ServerId,
                        principalTable: "Servers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SubstitutionRequests_AcceptedById",
                table: "SubstitutionRequests",
                column: "AcceptedById");

            migrationBuilder.CreateIndex(
                name: "IX_SubstitutionRequests_ChannelId",
                table: "SubstitutionRequests",
                column: "ChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_SubstitutionRequests_ServerId",
                table: "SubstitutionRequests",
                column: "ServerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SubstitutionRequests");
        }
    }
}

﻿// <auto-generated />
using System;
using CoachBot.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CoachBot.Domain.Migrations
{
    [DbContext(typeof(CoachBotContext))]
    [Migration("20191026183218_Initi21a55l711411155551814941")]
    partial class Initi21a55l711411155551814941
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.1-servicing-10028")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("CoachBot.Domain.Model.Channel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("BadgeEmote");

                    b.Property<string>("Color");

                    b.Property<DateTime>("CreatedDate");

                    b.Property<bool>("DisableSearchNotifications");

                    b.Property<decimal>("DiscordChannelId")
                        .HasConversion(new ValueConverter<decimal, decimal>(v => default(decimal), v => default(decimal), new ConverterMappingHints(precision: 20, scale: 0)));

                    b.Property<string>("DiscordChannelName");

                    b.Property<int>("Formation");

                    b.Property<int>("GuildId");

                    b.Property<bool>("IsMixChannel");

                    b.Property<string>("KitEmote");

                    b.Property<string>("Name");

                    b.Property<int?>("RegionId");

                    b.Property<DateTime>("UpdatedDate");

                    b.Property<bool>("UseClassicLineup");

                    b.HasKey("Id");

                    b.HasIndex("DiscordChannelId")
                        .IsUnique();

                    b.HasIndex("GuildId");

                    b.HasIndex("RegionId");

                    b.ToTable("Channels");
                });

            modelBuilder.Entity("CoachBot.Domain.Model.ChannelPosition", b =>
                {
                    b.Property<int>("ChannelId");

                    b.Property<int>("PositionId");

                    b.HasKey("ChannelId", "PositionId");

                    b.HasIndex("PositionId");

                    b.ToTable("ChannelPositions");
                });

            modelBuilder.Entity("CoachBot.Domain.Model.Match", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreatedDate");

                    b.Property<DateTime?>("ReadiedDate");

                    b.Property<int?>("ServerId");

                    b.Property<int?>("TeamAwayId");

                    b.Property<int?>("TeamHomeId");

                    b.HasKey("Id");

                    b.HasIndex("ServerId");

                    b.HasIndex("TeamAwayId");

                    b.HasIndex("TeamHomeId");

                    b.ToTable("Matches");
                });

            modelBuilder.Entity("CoachBot.Domain.Model.PlayerTeamPosition", b =>
                {
                    b.Property<int>("PlayerId");

                    b.Property<int>("PositionId");

                    b.Property<int>("TeamId");

                    b.HasKey("PlayerId", "PositionId", "TeamId");

                    b.HasIndex("TeamId");

                    b.HasIndex("PositionId", "TeamId")
                        .IsUnique();

                    b.ToTable("PlayerTeamPositions");
                });

            modelBuilder.Entity("CoachBot.Domain.Model.PlayerTeamSubstitute", b =>
                {
                    b.Property<int>("PlayerId");

                    b.Property<int>("TeamId");

                    b.HasKey("PlayerId", "TeamId");

                    b.HasIndex("TeamId");

                    b.ToTable("PlayerTeamSubstitute");
                });

            modelBuilder.Entity("CoachBot.Domain.Model.Search", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("ChannelId");

                    b.Property<DateTime>("CreatedDate");

                    b.Property<string>("DiscordMessageIds");

                    b.HasKey("Id");

                    b.HasIndex("ChannelId");

                    b.ToTable("Searches");
                });

            modelBuilder.Entity("CoachBot.Model.Guild", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<decimal>("DiscordGuildId")
                        .HasConversion(new ValueConverter<decimal, decimal>(v => default(decimal), v => default(decimal), new ConverterMappingHints(precision: 20, scale: 0)));

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Guilds");
                });

            modelBuilder.Entity("CoachBot.Model.Player", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<decimal?>("DiscordUserId")
                        .HasConversion(new ValueConverter<decimal, decimal>(v => default(decimal), v => default(decimal), new ConverterMappingHints(precision: 20, scale: 0)));

                    b.Property<string>("DiscordUserMention");

                    b.Property<string>("Name");

                    b.Property<string>("SteamID");

                    b.HasKey("Id");

                    b.ToTable("Players");
                });

            modelBuilder.Entity("CoachBot.Model.Position", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Positions");
                });

            modelBuilder.Entity("CoachBot.Model.Region", b =>
                {
                    b.Property<int>("RegionId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("RegionName");

                    b.HasKey("RegionId");

                    b.HasIndex("RegionName")
                        .IsUnique()
                        .HasFilter("[RegionName] IS NOT NULL");

                    b.ToTable("Regions");
                });

            modelBuilder.Entity("CoachBot.Model.Server", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Address");

                    b.Property<string>("Name");

                    b.Property<string>("RconPassword");

                    b.Property<int>("RegionId");

                    b.HasKey("Id");

                    b.HasIndex("Address")
                        .IsUnique()
                        .HasFilter("[Address] IS NOT NULL");

                    b.HasIndex("RegionId");

                    b.ToTable("Servers");
                });

            modelBuilder.Entity("CoachBot.Model.Team", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("ChannelId");

                    b.Property<int>("TeamType");

                    b.HasKey("Id");

                    b.HasIndex("ChannelId");

                    b.ToTable("Teams");
                });

            modelBuilder.Entity("CoachBot.Domain.Model.Channel", b =>
                {
                    b.HasOne("CoachBot.Model.Guild", "Guild")
                        .WithMany()
                        .HasForeignKey("GuildId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("CoachBot.Model.Region", "Region")
                        .WithMany()
                        .HasForeignKey("RegionId");
                });

            modelBuilder.Entity("CoachBot.Domain.Model.ChannelPosition", b =>
                {
                    b.HasOne("CoachBot.Domain.Model.Channel", "Channel")
                        .WithMany("ChannelPositions")
                        .HasForeignKey("ChannelId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("CoachBot.Model.Position", "Position")
                        .WithMany("ChannelPositions")
                        .HasForeignKey("PositionId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("CoachBot.Domain.Model.Match", b =>
                {
                    b.HasOne("CoachBot.Model.Server", "Server")
                        .WithMany()
                        .HasForeignKey("ServerId");

                    b.HasOne("CoachBot.Model.Team", "TeamAway")
                        .WithMany()
                        .HasForeignKey("TeamAwayId");

                    b.HasOne("CoachBot.Model.Team", "TeamHome")
                        .WithMany()
                        .HasForeignKey("TeamHomeId");
                });

            modelBuilder.Entity("CoachBot.Domain.Model.PlayerTeamPosition", b =>
                {
                    b.HasOne("CoachBot.Model.Player", "Player")
                        .WithMany("PlayerTeamPositions")
                        .HasForeignKey("PlayerId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("CoachBot.Model.Position", "Position")
                        .WithMany("PlayerTeamPositions")
                        .HasForeignKey("PositionId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("CoachBot.Model.Team", "Team")
                        .WithMany("PlayerTeamPositions")
                        .HasForeignKey("TeamId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("CoachBot.Domain.Model.PlayerTeamSubstitute", b =>
                {
                    b.HasOne("CoachBot.Model.Player", "Player")
                        .WithMany("PlayerSubstitutes")
                        .HasForeignKey("PlayerId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("CoachBot.Model.Team", "Team")
                        .WithMany("PlayerSubstitutes")
                        .HasForeignKey("TeamId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("CoachBot.Domain.Model.Search", b =>
                {
                    b.HasOne("CoachBot.Domain.Model.Channel", "Channel")
                        .WithMany()
                        .HasForeignKey("ChannelId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("CoachBot.Model.Server", b =>
                {
                    b.HasOne("CoachBot.Model.Region", "Region")
                        .WithMany()
                        .HasForeignKey("RegionId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("CoachBot.Model.Team", b =>
                {
                    b.HasOne("CoachBot.Domain.Model.Channel", "Channel")
                        .WithMany()
                        .HasForeignKey("ChannelId");
                });
#pragma warning restore 612, 618
        }
    }
}

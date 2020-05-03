import { Component, OnInit, Input } from '@angular/core';
import { DiscordService } from '../../shared/services/discord.service';
import { DiscordGuild } from '../../shared/model/discord-guild.model';
import { TeamService } from '../../shared/services/team.service';
import { GuildService } from '../../shared/services/guild.service';
import { Channel } from '../../shared/model/channel.model';
import { ChannelService } from '../../shared/services/channel.service';

@Component({
    selector: 'app-team-editor-discord-integration',
    templateUrl: './team-editor-discord-integration.component.html'
})
export class TeamEditorDiscordIntegrationComponent implements OnInit {

    @Input() teamId: number;
    isDiscordGuildSet = false;
    discordGuildId: string;
    discordGuilds: DiscordGuild[];
    channels: Channel[];
    isChannelWizardOpen = false;
    isLoading = true;

    constructor(
        private discordService: DiscordService,
        private teamService: TeamService,
        private guildService: GuildService,
        private channelService: ChannelService
    ) { }

    ngOnInit() {
        console.log(this.teamId);
        this.discordService.getGuildsForUser().subscribe(discordGuilds => {
            this.discordGuilds = discordGuilds;
            this.isLoading = false;
        });
    }

    loadChannels() {
        this.isLoading = true;
        this.channelService.getChannelsForGuild(this.discordGuildId).subscribe(channels => {
            this.channels = channels;
            this.isLoading = false;
        });
    }

    updateDiscordGuildId() {
        this.isLoading = true;
        this.guildService.getGuildByDiscordId(this.discordGuildId).subscribe(guildId => {
            this.teamService.updateGuildId(this.teamId, guildId.id).subscribe(() => {
                this.isLoading = false;
                this.isDiscordGuildSet = true;
            });
        });
    }
    
    
}

import { Component, Input, OnChanges } from '@angular/core';
import { TeamService } from '../../shared/services/team.service';
import { Channel } from '../../shared/model/channel.model';
import { ChannelService } from '../../shared/services/channel.service';
import { Team } from '../../shared/model/team.model';

@Component({
    selector: 'app-team-editor-discord-integration',
    templateUrl: './team-editor-discord-integration.component.html'
})
export class TeamEditorDiscordIntegrationComponent implements OnChanges {

    @Input() teamId: number;
    team: Team;
    channels: Channel[];
    isDiscordGuildSet: boolean;
    isGuildEditorOpen = false;
    isChannelWizardOpen = false;
    isLoading = true;

    constructor(private teamService: TeamService, private channelService: ChannelService) { }

    ngOnChanges() {
        if (this.teamId) {
            this.teamService.getTeam(this.teamId).subscribe(team => {
                this.team = team;
                if (this.team.guild && this.team.guild.discordGuildId) {
                    this.isDiscordGuildSet = true;
                    this.loadChannels();
                }
            });
        }
    }

    loadChannels() {
        this.isLoading = true;
        this.channelService.getChannelsForGuild(this.team.guild.discordGuildId).subscribe(channels => {
            this.channels = channels;
            this.isLoading = false;
        });
    }

}

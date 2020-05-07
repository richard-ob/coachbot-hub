import { Component, Output, EventEmitter } from '@angular/core';
import { DiscordService } from '../../../shared/services/discord.service';
import { TeamService } from '../../../shared/services/team.service';
import { DiscordGuild } from '../../../shared/model/discord-guild.model';
import { GuildService } from '../../../shared/services/guild.service';
import { Team } from '../../../shared/model/team.model';

@Component({
    selector: 'app-discord-guild-editor',
    templateUrl: './discord-guild-editor.component.html'
})
export class DiscordGuildEditorComponent {

    @Output() guildEditorClosed = new EventEmitter<void>();
    discordGuilds: DiscordGuild[];
    teamId: number;
    team: Team;
    selectedDiscordGuildId: string;
    discordGuildPreviouslySet = false;
    isEditing = false;
    isLoading = true;

    constructor(private discordService: DiscordService, private teamService: TeamService, private guildService: GuildService) { }

    openGuildEditor(teamId: number, discordGuildId: string) {
        this.isLoading = true;
        this.isEditing = true;
        this.teamId = teamId;
        this.selectedDiscordGuildId = discordGuildId;
        this.discordGuildPreviouslySet = discordGuildId != null;
        this.discordService.getGuildsForUser().subscribe(discordGuilds => {
            this.discordGuilds = discordGuilds;
            this.teamService.getTeam(teamId).subscribe(team => {
                this.team = team;
                this.isLoading = false;
            });
        });
    }

    closeEditor() {
        this.isEditing = false;
        this.guildEditorClosed.emit();
    }

    updateDiscordGuild() {
        this.isLoading = true;
        this.guildService.getGuildByDiscordId(this.selectedDiscordGuildId).subscribe(guild => {
            this.team.guildId = guild.id;
            this.teamService.updateTeam(this.team).subscribe(() => {
                this.isLoading = false;
                this.isEditing = false;
                this.guildEditorClosed.emit();
            });
        });
    }

}

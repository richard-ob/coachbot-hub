import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { DiscordService } from '../../../shared/services/discord.service';
import { TeamService } from '../../../shared/services/team.service';
import { GuildService } from '../../../shared/services/guild.service';
import { ChannelService } from '../../../shared/services/channel.service';
import { DiscordChannel } from '../../../shared/model/discord-channel.model';
import { Channel } from '../../../shared/model/channel.model';
import { Team } from '../../../shared/model/team.model';

@Component({
    selector: 'app-channel-editor',
    templateUrl: './channel-editor.component.html'
})
export class ChannelEditorComponent implements OnInit {

    @Output() wizardClosed = new EventEmitter<void>();
    team: Team;
    channel: Channel = new Channel();
    discordChannels: DiscordChannel[];
    discordGuildId: string;
    wizardMode: WizardMode;
    wizardStep: WizardStep;
    wizardModes = WizardMode;
    wizardSteps = WizardStep;
    isLoading = true;
    isSaving = false;

    constructor(
        private discordService: DiscordService,
        private teamService: TeamService,
        private guildService: GuildService,
        private channelService: ChannelService
    ) { }

    ngOnInit() {

    }

    startCreateWizard(teamId: number) {
        this.isLoading = true;
        this.wizardMode = WizardMode.Creating;
        this.wizardStep = WizardStep.ChannelSelection;
        this.channel = new Channel();
        this.channel.teamId = teamId;
        this.teamService.getTeam(teamId).subscribe(team => {
            this.team = team;
            this.discordService.getChannelsForGuild(this.team.guild.discordGuildId).subscribe(discordChannels => {
                this.discordChannels = discordChannels;
                this.isLoading = false;
            });
        });
    }

    startEditWizard(channel: number) {
        this.isSaving = true;
        this.wizardMode = WizardMode.Editing;
    }

    abortWizard() {
        this.wizardMode = WizardMode.Disabled;
        this.isLoading = true;
        this.isSaving = false;
        this.wizardClosed.emit();
    }

    saveChannel() {
        this.isLoading = true;
        switch (this.wizardMode) {
            case WizardMode.Creating:
                this.channelService.createChannel(this.channel).subscribe(() => {
                    this.isLoading = false;
                    this.abortWizard();
                });
                break;
            case WizardMode.Editing:
                this.channelService.updateChannel(this.channel).subscribe(() => {
                    this.isLoading = false;
                    this.abortWizard();
                });
                break;
        }
    }

}

enum WizardMode {
    Disabled,
    Creating,
    Editing
}
enum WizardStep {
    ChannelSelection,
    Configuration
}

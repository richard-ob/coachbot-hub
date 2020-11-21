import { Component, Output, EventEmitter, ViewChild, OnInit, Input } from '@angular/core';
import { DiscordService } from '../../../shared/services/discord.service';
import { TeamService } from '../../../shared/services/team.service';
import { ChannelService } from '../../../shared/services/channel.service';
import { DiscordChannel } from '../../../shared/model/discord-channel.model';
import { Channel } from '../../../shared/model/channel.model';
import { Team } from '../../../shared/model/team.model';
import { Position } from '../../../shared/model/position';
import { ChannelPosition } from '../../../shared/model/channel-position';
import { FormatPositions } from './format-positions';
import { SwalComponent, SwalPortalTargets } from '@sweetalert2/ngx-sweetalert2';
import { UserPreferenceService, UserPreferenceType } from '@shared/services/user-preferences.service';
import { Observable } from 'rxjs';
import { distinctUntilChanged, debounceTime, map } from 'rxjs/operators';
import { PlayerService } from '@pages/hub/shared/services/player.service';
import { Player } from '@pages/hub/shared/model/player.model';
import { PlayerHubRole } from '@pages/hub/shared/model/player-hub-role.enum';
import { ChannelType } from '@pages/hub/shared/model/channel-type.enum';
import StringUtils from '@shared/utilities/string-utilities';
import { MatchFormat } from '@pages/hub/shared/model/match-format.enum';

@Component({
    selector: 'app-discord-channel-editor',
    templateUrl: './discord-channel-editor.component.html',
    styleUrls: ['./discord-channel-editor.component.scss']
})
export class DiscordChannelEditorComponent implements OnInit {

    @ViewChild('editPositionModal') editPositionModal: SwalComponent;
    @Output() wizardClosed = new EventEmitter<void>();
    @Input() channels: Channel[];
    @Input() team: Team;
    currentPlayer: Player;
    hubRoles = PlayerHubRole;
    teams: Team[];
    channel: Channel = new Channel();
    isMixChannel: boolean;
    channelTypes = ChannelType;
    discordChannels: DiscordChannel[];
    discordGuildId: string;
    positionToEdit: ChannelPosition;
    wizardMode: WizardMode;
    wizardStep: WizardStep;
    wizardModes = WizardMode;
    wizardSteps = WizardStep;
    isLoading = true;
    isSaving = false;
    selectedIgnoreTeam: Team;
    search = (text$: Observable<string>) =>
        text$.pipe(
            debounceTime(200),
            distinctUntilChanged(),
            map(term => term.length < 1 ? [] : this.teams.filter(v =>
                v.name.toLowerCase().indexOf(term.toLowerCase()) > -1 || v.teamCode.toLowerCase().indexOf(term.toLowerCase()) > -1
            ).slice(0, 10))
        )
    formatter = (x: { name: string }) => x.name;

    constructor(
        private discordService: DiscordService,
        private teamService: TeamService,
        private channelService: ChannelService,
        private playerService: PlayerService,
        public readonly swalTargets: SwalPortalTargets,
        private userPreferenceService: UserPreferenceService
    ) { }

    ngOnInit() {
        this.currentPlayer = this.playerService.currentPlayer;
        this.teamService.getTeams(this.userPreferenceService.getUserPreference(UserPreferenceType.Region)).subscribe(teams => {
            this.teams = teams;
        });
    }

    startCreateWizard(teamId: number) {
        this.isLoading = true;
        this.wizardMode = WizardMode.Creating;
        this.wizardStep = WizardStep.ChannelSelection;
        this.channel = new Channel();
        this.channel.teamId = teamId;
        this.isMixChannel = false;
        this.setPositions(MatchFormat.EightVsEight);
        this.discordService.getChannelsForGuild(this.team.guild.discordGuildId).subscribe(discordChannels => {
            this.discordChannels = discordChannels;
            this.isLoading = false;
        });
    }

    startEditWizard(channelId: number) {
        this.isLoading = true;
        this.wizardMode = WizardMode.Editing;
        this.wizardStep = WizardStep.Configuration;
        this.channelService.getChannel(channelId).subscribe(channel => {
            this.channel = channel;
            this.isMixChannel = channel.channelType !== ChannelType.Team;
            this.channel.channelPositions = channel.channelPositions.sort((a, b) => a.ordinal - b.ordinal);
            this.isLoading = false;
        });
    }

    abortWizard() {
        this.wizardMode = WizardMode.Disabled;
        this.isLoading = true;
        this.isSaving = false;
        this.selectedIgnoreTeam = null;
        this.wizardClosed.emit();
    }

    setChannelName(discordChannelId: string) {
        this.channel.discordChannelName = this.discordChannels.find(c => c.id === discordChannelId).name;
    }

    setChannelType() {
        this.channel.channelType = this.isMixChannel ? ChannelType.PrivateMix : ChannelType.Team;
    }

    setPositions(format: number) {
        this.channel.channelPositions = this.channel.channelPositions || [];
        if (this.channel.channelPositions.length > format) {
            this.channel.channelPositions.splice(format);
        } else if (this.channel.channelPositions.length < format) {
            while (this.channel.channelPositions.length < format) {
                const channelPosition = new ChannelPosition();
                channelPosition.position = new Position();
                channelPosition.channelId = this.channel.id;
                this.channel.channelPositions.push(channelPosition);
            }
        }
        this.usePositionNames();
    }

    usePositionNames() {
        const format = this.channel.channelPositions.length;

        const positionNames = FormatPositions.names;
        for (let i = 0; i < format; i++) {
            const channelPosition = this.channel.channelPositions[i];
            channelPosition.positionId = 0;
            channelPosition.position = new Position();
            channelPosition.position.id = 0;
            channelPosition.position.name = positionNames[format][i];
            channelPosition.ordinal = i + 1;
        }
    }

    usePositionNumbersWithGk() {
        const firstChannelPosition = this.channel.channelPositions[0];
        firstChannelPosition.positionId = 0;
        firstChannelPosition.position = new Position();
        firstChannelPosition.position.id = 0;
        firstChannelPosition.position.name = 'GK';
        firstChannelPosition.ordinal = 0;
        const format = this.channel.channelPositions.length;
        for (let i = 1; i < format; i++) {
            const channelPosition = this.channel.channelPositions[i];
            channelPosition.positionId = 0;
            channelPosition.position = new Position();
            channelPosition.position.id = 0;
            channelPosition.position.name = i.toString();
            channelPosition.ordinal = i;
        }
    }

    usePositionNumbers() {
        const format = this.channel.channelPositions.length;
        for (let i = 1; i <= format; i++) {
            const channelPosition = this.channel.channelPositions[i - 1];
            channelPosition.positionId = 0;
            channelPosition.position = new Position();
            channelPosition.position.id = 0;
            channelPosition.position.name = i.toString();
            channelPosition.ordinal = i;
        }
    }

    editPosition(channelPosition: ChannelPosition) {
        this.positionToEdit = channelPosition;
        this.editPositionModal.fire();
    }

    updatePosition() {
        this.positionToEdit.positionId = 0;
        this.positionToEdit.position.id = 0;
        this.editPositionModal.dismiss();
    }

    addSearchIgnoreChannel() {
        if (this.selectedIgnoreTeam && this.selectedIgnoreTeam.id) {
            this.channel.searchIgnoreList.push(this.selectedIgnoreTeam.id);
            this.selectedIgnoreTeam = null;
        }
    }

    removeSearchIgnoreChannel(teamId: number) {
        this.channel.searchIgnoreList = this.channel.searchIgnoreList.filter(t => t !== teamId);
    }

    isDuplicateTeamCode() {
        return this.channels.some(c => StringUtils.upperCase(this.channel.subTeamCode) === StringUtils.upperCase(c.subTeamCode)
            && c.id !== this.channel.id && c.channelPositions.length === this.channel.channelPositions.length);
    }

    saveChannel() {
        this.isLoading = true;
        this.isSaving = true;
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

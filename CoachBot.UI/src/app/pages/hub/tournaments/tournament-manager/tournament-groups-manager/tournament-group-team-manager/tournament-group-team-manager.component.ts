import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { UserPreferenceService, UserPreferenceType } from '@shared/services/user-preferences.service';
import { TournamentGroupTeamDto } from '@pages/hub/shared/model/dtos/tournament-group-team-dto.model';
import { Team } from '@pages/hub/shared/model/team.model';
import { TournamentService } from '@pages/hub/shared/services/tournament.service';
import { TeamService } from '@pages/hub/shared/services/team.service';

@Component({
    selector: 'app-tournament-group-team-manager',
    templateUrl: './tournament-group-team-manager.component.html'
})
export class TournamentGroupTeamManagerComponent implements OnInit {

    @Input() tournamentGroupId: number;
    @Output() teamAdded = new EventEmitter<void>();
    tournamentGroupTeamDto: TournamentGroupTeamDto = new TournamentGroupTeamDto();
    teams: Team[];
    isSaving = false;
    isLoading = true;

    constructor(
        private tournamentService: TournamentService,
        private teamService: TeamService,
        private userPreferencsService: UserPreferenceService
    ) { }

    ngOnInit() {
        const regionId = this.userPreferencsService.getUserPreference(UserPreferenceType.Region);
        this.teamService.getTeams(regionId).subscribe(teams => {
            this.teams = teams;
            this.isLoading = false;
        });
    }

    addTeam() {
        this.isLoading = true;
        this.tournamentGroupTeamDto.tournamentGroupId = this.tournamentGroupId;
        this.tournamentService.addTournamentGroupTeam(this.tournamentGroupTeamDto).subscribe(() => {
            this.isLoading = false;
            this.teamAdded.emit();
            this.tournamentGroupTeamDto = new TournamentGroupTeamDto();
        });
    }

}

import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { TournamentGroupTeamDto } from '../../../shared/model/dtos/tournament-group-team-dto.model';
import { TournamentService } from '../../../shared/services/tournament.service';
import { TeamService } from '../../../shared/services/team.service';
import { Team } from '../../../shared/model/team.model';

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

    constructor(private tournamentService: TournamentService, private teamService: TeamService) { }

    ngOnInit() {
        this.teamService.getTeams().subscribe(teams => {
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

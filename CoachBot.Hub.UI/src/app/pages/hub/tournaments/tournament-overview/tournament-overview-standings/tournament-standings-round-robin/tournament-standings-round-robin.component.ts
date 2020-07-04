import { Component, OnInit, Input } from '@angular/core';
import { Tournament } from '@pages/hub/shared/model/tournament.model';
import { TournamentGroupMatch } from '@pages/hub/shared/model/tournament-group-match.model';
import { TeamStatistics } from '@pages/hub/shared/model/team-statistics.model';
import { TeamStatisticFilters } from '@pages/hub/shared/model/dtos/paged-team-statistics-request-dto.model';
import { TeamService } from '@pages/hub/shared/services/team.service';

@Component({
    selector: 'app-tournament-standings-round-robin',
    templateUrl: './tournament-standings-round-robin.component.html'
})
export class TournamentStandingsRoundRobinComponent implements OnInit {

    @Input() tournament: Tournament;
    teamStatistics: TeamStatistics[];
    filters: TeamStatisticFilters = new TeamStatisticFilters();
    isLoading = true;
    matches: TournamentGroupMatch;

    constructor(private teamService: TeamService) { }

    ngOnInit() {
        this.filters.tournamentId = this.tournament.id;
        this.teamService.getTeamStatistics(1, 100, 'Points', 'DESC', this.filters)
            .subscribe(response => {
                this.teamStatistics = response.items;
                this.isLoading = false;
            });
    }
}

import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { PlayerTeamStatisticsTotals } from '../../../shared/model/player-team-statistics-totals.model';
import { TeamService } from '../../../shared/services/team.service';
import { TeamStatistics } from '../../../shared/model/team-statistics.model';

@Component({
    selector: 'app-tournament-overview-standings',
    templateUrl: './tournament-overview-standings.component.html'
})
export class TournamentOverviewStandingsComponent implements OnInit {

    tournamentEditionId: number;
    teamStatistics: TeamStatistics[];
    isLoading = true;

    constructor(private route: ActivatedRoute, private teamService: TeamService) { }

    ngOnInit() {
        this.route.parent.paramMap.pipe().subscribe(params => {
            this.tournamentEditionId = +params.get('id');
            this.teamService.getTeamStatistics(1, 100, this.tournamentEditionId, undefined, 'Points', 'DESC').subscribe(response => {
                this.teamStatistics = response.items;
                this.isLoading = false;
            });
        });
    }

}

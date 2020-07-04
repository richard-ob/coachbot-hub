import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { TeamStatistics } from '../../../shared/model/team-statistics.model';
import { TeamStatisticFilters } from '@pages/hub/shared/model/dtos/paged-team-statistics-request-dto.model';
import { Tournament } from '@pages/hub/shared/model/tournament.model';
import { TournamentService } from '@pages/hub/shared/services/tournament.service';
import { TournamentType } from '@pages/hub/shared/model/tournament-type.enum';

@Component({
    selector: 'app-tournament-overview-standings',
    templateUrl: './tournament-overview-standings.component.html'
})
export class TournamentOverviewStandingsComponent implements OnInit {

    tournamentId: number;
    tournament: Tournament;
    teamStatistics: TeamStatistics[];
    filters: TeamStatisticFilters = new TeamStatisticFilters();
    tournamentTypes = TournamentType;
    isLoading = true;

    constructor(private route: ActivatedRoute, private tournamentService: TournamentService) { }

    ngOnInit() {
        this.route.parent.paramMap.pipe().subscribe(params => {
            this.tournamentId = +params.get('id');
            this.filters.tournamentId = this.tournamentId;
            this.tournamentService.getTournament(this.tournamentId).subscribe(tournament => {
                this.tournament = tournament;
                this.isLoading = false;
            });
        });
    }

}

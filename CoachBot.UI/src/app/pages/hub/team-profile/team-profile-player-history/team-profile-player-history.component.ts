import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { TeamService } from '../../shared/services/team.service';
import { PlayerTeamStatisticsTotals } from '../../shared/model/player-team-statistics-totals.model';

@Component({
    selector: 'app-team-profile-player-history',
    templateUrl: './team-profile-player-history.component.html'
})
export class TeamProfilePlayerHistoryComponent implements OnInit {

    playerTeamStatisticsTotals: PlayerTeamStatisticsTotals[];
    isLoading = true;

    constructor(private route: ActivatedRoute, private teamService: TeamService) { }

    ngOnInit() {
        this.route.parent.paramMap.pipe().subscribe(params => {
            const teamId = +params.get('id');
            this.teamService.getTeamPlayerHistory(teamId).subscribe((playerTeamStatisticsTotals) => {
                this.playerTeamStatisticsTotals = playerTeamStatisticsTotals;
            });
        });
    }

}

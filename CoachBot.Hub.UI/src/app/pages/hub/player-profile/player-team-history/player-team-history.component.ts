import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { PlayerService } from '../../shared/services/player.service';
import { PlayerTeamStatisticsTotals } from '../../shared/model/player-team-statistics-totals.model';

@Component({
    selector: 'app-player-team-history',
    templateUrl: './player-team-history.component.html'
})
export class PlayerTeamHistoryComponent implements OnInit {

    playerTeamStatisticsTotals: PlayerTeamStatisticsTotals[];
    isLoading = true;

    constructor(private route: ActivatedRoute, private playerService: PlayerService) { }

    ngOnInit() {
        this.route.parent.paramMap.pipe().subscribe(params => {
            const playerId = +params.get('id');
            this.playerService.getPlayerTeamStatisticsHistory(playerId).subscribe((playerTeamStatisticsTotals) => {
                this.playerTeamStatisticsTotals = playerTeamStatisticsTotals;
                this.isLoading = false;
            });
        });
    }
}

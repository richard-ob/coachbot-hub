import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FantasyService } from '@pages/hub/shared/services/fantasy.service';
import { FantasyTeamRank } from '@pages/hub/shared/model/fantasy-team-rank.model';
import { PlayerService } from '@pages/hub/shared/services/player.service';
import { Player } from '@pages/hub/shared/model/player.model';

@Component({
    selector: 'app-fantasy-overview',
    templateUrl: './fantasy-overview.component.html',
    styleUrls: ['./fantasy-overview.component.scss'],
    encapsulation: ViewEncapsulation.None
})
export class FantasyOverviewComponent implements OnInit {

    tournamentId: number;
    fantasyTeamRankings: FantasyTeamRank[];
    currentPlayer: Player;
    isCreating = false;
    isLoading = true;

    constructor(
        private fantasyService: FantasyService,
        private playerService: PlayerService,
        private route: ActivatedRoute,
        private router: Router) { }

    ngOnInit() {
        this.route.paramMap.pipe().subscribe(params => {
            this.tournamentId = +params.get('tournamentId');
            this.fantasyService.getFantasyTeamRankings(this.tournamentId).subscribe(rankings => {
                this.fantasyTeamRankings = rankings;
                this.playerService.getCurrentPlayer().subscribe(player => {
                    this.currentPlayer = player;
                    this.isLoading = false;
                });
            });
        });
    }

}

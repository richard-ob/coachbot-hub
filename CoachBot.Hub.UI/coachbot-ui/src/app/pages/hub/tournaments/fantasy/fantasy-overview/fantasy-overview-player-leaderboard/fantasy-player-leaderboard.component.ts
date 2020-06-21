import { Component, OnInit, Input } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FantasyService } from '@pages/hub/shared/services/fantasy.service';
import { FantasyPlayerRank } from '@pages/hub/shared/model/fantasy-player-rank';

@Component({
    selector: 'app-fantasy-player-leaderboard',
    templateUrl: './fantasy-player-leaderboard.component.html'
})
export class FantasyPlayerLeaderboardComponent implements OnInit {

    @Input() tournamentId: number;
    @Input() currentPlayerId: number;
    fantasyPlayerRankings: FantasyPlayerRank[];
    isCreating = false;
    isLoading = true;

    constructor(private fantasyService: FantasyService, private route: ActivatedRoute, private router: Router) { }

    ngOnInit() {
        this.fantasyService.getFantasyPlayerRankings(this.tournamentId).subscribe(rankings => {
            this.fantasyPlayerRankings = rankings;
            this.isLoading = false;
        });
    }

}

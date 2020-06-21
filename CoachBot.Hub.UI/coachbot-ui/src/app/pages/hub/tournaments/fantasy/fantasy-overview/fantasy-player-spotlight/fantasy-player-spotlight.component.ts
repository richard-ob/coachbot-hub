import { Component, OnInit, Input, ViewEncapsulation } from '@angular/core';
import { FantasyService } from '@pages/hub/shared/services/fantasy.service';
import { FantasyPlayerRank } from '@pages/hub/shared/model/fantasy-player-rank';
import { Router } from '@angular/router';

@Component({
    selector: 'app-fantasy-player-spotlight',
    templateUrl: './fantasy-player-spotlight.component.html',
    styleUrls: ['./fantasy-player-spotlight.component.scss'],
    encapsulation: ViewEncapsulation.None
})
export class FantasyPlayerSpotlightComponent implements OnInit {

    @Input() tournamentId: number;
    spotlightPlayer: FantasyPlayerRank;
    isCreating = false;
    isLoading = true;

    constructor(
        private fantasyService: FantasyService,
        private router: Router
    ) { }

    ngOnInit() {
        this.fantasyService.getFantasyPlayerSpotlight(this.tournamentId).subscribe(spotlightPlayer => {
            this.spotlightPlayer = spotlightPlayer;
            this.isLoading = false;
        });
    }

    navigateToPlayer(playerId: number) {
        this.router.navigate(['/player-profile/', playerId]);
    }

}

import { Component, OnInit } from '@angular/core';
import { PlayerService } from '@pages/hub/shared/services/player.service';
import { Player } from '@pages/hub/shared/model/player.model';

@Component({
    selector: 'app-score-predictor-hub',
    templateUrl: './score-predictor-hub.component.html'
})
export class ScorePredictorHubComponent implements OnInit {

    player: Player;
    isLoading = true;

    constructor(private playerService: PlayerService) { }

    ngOnInit() {
        this.playerService.getCurrentPlayer().subscribe(player => {
            this.player = player;
            this.isLoading = false;
        });
    }
}

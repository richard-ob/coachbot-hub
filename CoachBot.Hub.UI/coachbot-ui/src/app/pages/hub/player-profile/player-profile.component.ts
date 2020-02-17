import { Component, OnInit } from '@angular/core';
import { Player } from '../shared/model/player.model';
import { PlayerService } from '../shared/services/player.service';

@Component({
    selector: 'app-player-profile',
    templateUrl: './player-profile.component.html',
    styleUrls: ['./player-profile.component.scss']
})
export class PlayerProfileComponent implements OnInit {

    player: Player;
    currentPage = 1;
    totalPages: number;
    totalItems: number;

    constructor(private playerService: PlayerService) {

    }

    ngOnInit() {
        this.loadPage(1);
    }

    loadPage(playerId: number) {
        this.playerService.getPlayer(1).subscribe(response => {
            this.player = response;
        });
    }

}

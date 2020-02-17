import { Component, OnInit } from '@angular/core';
import { Player } from '../shared/model/player.model';
import { PlayerService } from '../shared/services/player.service';

@Component({
    selector: 'app-player-list',
    templateUrl: './player-list.component.html',
    styleUrls: ['./player-list.component.scss']
})
export class PlayerListComponent implements OnInit {

    players: Player[];
    currentPage = 1;
    totalPages: number;
    totalItems: number;

    constructor(private playerService: PlayerService) {

    }

    ngOnInit() {
        this.loadPage(1);
    }

    loadPage(page: number) {
        this.playerService.getPlayers(page).subscribe(response => {
            this.players = response.items;
            this.currentPage = response.page;
            this.totalPages = response.totalPages;
            this.totalItems = response.totalItems;
        });
    }

}

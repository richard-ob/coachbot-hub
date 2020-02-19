import { Component, OnInit } from '@angular/core';
import { PlayerService } from '../shared/services/player.service';
import { PlayerStatistics } from '../shared/model/player-statistics.model';

@Component({
    selector: 'app-player-list',
    templateUrl: './player-list.component.html',
    styleUrls: ['./player-list.component.scss']
})
export class PlayerListComponent implements OnInit {

    playerStatistics: PlayerStatistics[];
    currentPage = 1;
    totalPages: number;
    totalItems: number;

    constructor(private playerService: PlayerService) {

    }

    ngOnInit() {
        this.loadPage(1);
    }

    loadPage(page: number) {
        this.playerService.getPlayerStatistics(page).subscribe(response => {
            this.playerStatistics = response.items;
            this.currentPage = response.page;
            this.totalPages = response.totalPages;
            this.totalItems = response.totalItems;
        });
    }

}

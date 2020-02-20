import { Component, OnInit } from '@angular/core';
import { PlayerService } from '../shared/services/player.service';
import { PlayerStatistics } from '../shared/model/player-statistics.model';
import * as SteamID from 'steamid';

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
    sortBy: string = null;
    sortOrder = 'ASC';
    timePeriod = 0;

    constructor(private playerService: PlayerService) {

    }

    ngOnInit() {
        this.loadPage(1);
    }

    loadPage(page: number, sortBy: string = null) {
        if (sortBy !== null && this.sortBy !== null && this.sortBy === sortBy && this.sortOrder === 'ASC') {
            this.sortOrder = 'DESC';
        } else {
            this.sortOrder = 'ASC';
        }
        this.sortBy = sortBy;
        this.playerService.getPlayerStatistics(page, this.sortBy, this.sortOrder, this.timePeriod).subscribe(response => {
            this.playerStatistics = response.items;
            this.currentPage = response.page;
            this.totalPages = response.totalPages;
            this.totalItems = response.totalItems;
        });
    }

    setTimePeriod(timePeriod: number) {
        this.timePeriod = timePeriod;
        this.loadPage(this.currentPage, this.sortBy);
    }

    getSteamProfileLink(steamId: string) {
        const steamUser = new SteamID(steamId);

        return `http://steamcommunity.com/profiles/${steamUser.getSteamID64()}`;

    }

}

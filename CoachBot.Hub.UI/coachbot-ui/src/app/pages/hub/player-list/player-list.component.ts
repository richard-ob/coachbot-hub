import { Component, OnInit } from '@angular/core';
import { PlayerService } from '../shared/services/player.service';
import { PlayerStatistics } from '../shared/model/player-statistics.model';
import { SteamService } from '../shared/services/steam.service.';

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
    isLoading = true;

    constructor(private playerService: PlayerService, private steamService: SteamService) { }

    ngOnInit() {
        this.loadPage(1);
    }

    loadPage(page: number, sortBy: string = null) {
        this.isLoading = true;
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
            this.isLoading = false;
            this.getSteamUserProfiles(this.playerStatistics);
        });
    }

    setTimePeriod(timePeriod: number) {
        this.timePeriod = timePeriod;
        this.loadPage(this.currentPage, this.sortBy);
    }

    getSteamProfileLink(steamId: string) {
        return `http://steamcommunity.com/profiles/${steamId}`;
    }

    getSteamUserProfiles(playerStatistics: PlayerStatistics[]) {
        const steamIds = [];
        for (const player of playerStatistics) {
            console.log(player.player);
            if (player.player.steamID) {
                steamIds.push(player.player.steamID);
            }
        }
        console.log(steamIds);
        this.steamService.getUserProfiles(steamIds).subscribe(response => {
            for (const player of playerStatistics) {
                if (player.player.steamID && player.player.steamID.length > 5) {
                    const steamUserProfile = response.response.players.find(u => u.steamid === player.player.steamID);
                    if (steamUserProfile) {
                        player.player.steamUserProfile = steamUserProfile;
                    }
                }
            }
        });
    }

}

import { Component, OnInit } from '@angular/core';
import { PlayerService } from '../shared/services/player.service';
import { PlayerStatistics } from '../shared/model/player-statistics.model';
import { SteamService } from '../../../shared/services/steam.service.';
import { Router } from '@angular/router';
import { PlayerStatisticFilters } from '../shared/model/dtos/paged-player-statistics-request-dto.model';

@Component({
    selector: 'app-player-list',
    templateUrl: './player-list.component.html',
    styleUrls: ['./player-list.component.scss']
})
export class PlayerListComponent implements OnInit {

    playerStatistics: PlayerStatistics[];
    filters = new PlayerStatisticFilters();
    currentPage = 1;
    totalPages: number;
    totalItems: number;
    sortBy: string = null;
    sortOrder = 'ASC';
    timePeriod = 0;
    isLoading = true;
    isLoadingPage = false;

    constructor(
        private playerService: PlayerService,
        private steamService: SteamService,
        private router: Router
    ) { }

    ngOnInit() {

        this.loadPage(1);
    }

    loadPage(page: number, sortBy: string = null) {
        this.isLoadingPage = true;
        if (sortBy !== null && this.sortBy !== null && this.sortBy === sortBy && this.sortOrder === 'ASC') {
            this.sortOrder = 'DESC';
        } else {
            this.sortOrder = 'ASC';
        }
        this.sortBy = sortBy;
        this.playerService.getPlayerStatistics(page, this.sortBy, this.sortOrder, this.filters).subscribe(response => {
            this.playerStatistics = response.items;
            this.currentPage = response.page;
            this.totalPages = response.totalPages;
            this.totalItems = response.totalItems;
            this.isLoading = false;
            this.isLoadingPage = false;
            this.getSteamUserProfiles(this.playerStatistics);
        });
    }

    setFilters() {
        this.loadPage(this.currentPage, this.sortBy);
    }

    getSteamProfileLink(steamId: string) {
        return `http://steamcommunity.com/profiles/${steamId}`;
    }

    getSteamUserProfiles(playerStatistics: PlayerStatistics[]) {
        const steamIds = [];
        for (const player of playerStatistics) {
            if (player.steamID) {
                steamIds.push(player.steamID);
            }
        }
        console.log(steamIds);
        this.steamService.getUserProfiles(steamIds).subscribe(response => {
            for (const player of playerStatistics) {
                if (player.steamID && player.steamID.length > 5) {
                    const steamUserProfile = response.response.players.find(u => u.steamid === player.steamID);
                    if (steamUserProfile) {
                        player.steamUserProfile = steamUserProfile;
                    }
                }
            }
        });
    }

    navigateToProfile(playerId: number) {
        this.router.navigate(['/player-profile', playerId]);
    }

}

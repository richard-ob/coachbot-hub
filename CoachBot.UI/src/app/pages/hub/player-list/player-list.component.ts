import { Component, OnInit, Input } from '@angular/core';
import { PlayerService } from '../shared/services/player.service';
import { PlayerStatistics } from '../shared/model/player-statistics.model';
import { SteamService } from '../../../shared/services/steam.service.';
import { Router } from '@angular/router';
import { PlayerStatisticFilters } from '../shared/model/dtos/paged-player-statistics-request-dto.model';
import SortingUtils from '@shared/utilities/sorting-utilities';
import { UserPreferenceService, UserPreferenceType } from '@shared/services/user-preferences.service';
import { Team } from '../shared/model/team.model';
import { TeamService } from '../shared/services/team.service';
import { PlayerSpotlightStatistic } from './player-spotlight/player-spotlight-statistic.enum';
import { PlayerStatType } from './player-stat-type.enum';
import { RegionService } from '../shared/services/region.service';

@Component({
    selector: 'app-player-list',
    templateUrl: './player-list.component.html',
    styleUrls: ['./player-list.component.scss']
})
export class PlayerListComponent implements OnInit {

    @Input() tournamentId: number;
    @Input() hideFilters = false;
    playerStatistics: PlayerStatistics[];
    filters = new PlayerStatisticFilters();
    teams: Team[];
    positions = ['GK', 'LB', 'CB', 'RB', 'LW', 'LM', 'CM', 'RM', 'RW', 'CF'];
    playerSpotlightStatistic = PlayerSpotlightStatistic;
    playerStatType = PlayerStatType;
    currentPlayerStat = PlayerStatType.General;
    includePartialAppearances = false;
    currentPage = 1;
    totalPages: number;
    totalItems: number;
    sortBy: string = null;
    sortOrder = 'ASC';
    timePeriod = 0;
    isLoading = true;
    isLoadingPage = false;
    filtersApplied = false;
    readonly PAGE_SIZE = 10;

    constructor(
        private playerService: PlayerService,
        private steamService: SteamService,
        private teamService: TeamService,
        private regionService: RegionService,
        private router: Router,
        private userPreferenceService: UserPreferenceService
    ) { }

    ngOnInit() {
        const regionId = this.userPreferenceService.getUserPreference(UserPreferenceType.Region);
        this.filters.tournamentId = this.tournamentId;
        this.filters.regionId = regionId;
        this.filters.includeSubstituteAppearances = false;
        this.setIncludePartialAppearances();
        this.regionService.getRegions().subscribe(regions => {
            const region = regions.find(r => r.regionId === regionId);
            this.filters.matchFormat = region.matchFormat;
            this.teamService.getTeams(regionId).subscribe(teams => {
                this.teams = teams;
                this.loadPage(1);
            });
        });
    }

    loadPage(page: number, sortBy: string = null) {
        this.isLoadingPage = true;
        if (page === this.currentPage) {
            this.sortOrder = SortingUtils.getSortOrder(this.sortBy, sortBy, this.sortOrder);
            this.sortBy = sortBy;
        }
        this.playerService.getPlayerStatistics(page, this.PAGE_SIZE, this.sortBy, this.sortOrder, this.filters).subscribe(response => {
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
        this.sortOrder = this.sortOrder === 'DESC' ? 'ASC' : 'DESC';
        this.loadPage(1, this.sortBy);
        this.filtersApplied = true;
    }

    setIncludePartialAppearances() {
        if (this.includePartialAppearances) {
            this.filters.minimumSecondsPlayed = null;
        } else {
            this.filters.minimumSecondsPlayed = 60 * 90;
        }
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

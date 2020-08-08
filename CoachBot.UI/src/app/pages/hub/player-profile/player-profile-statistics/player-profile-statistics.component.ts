import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { PlayerService } from '../../shared/services/player.service';
import { PlayerStatisticFilters } from '../../shared/model/dtos/paged-player-statistics-request-dto.model';
import { TimePeriod } from '../../shared/model/time-period.enum';
import { map } from 'rxjs/operators';
import { PlayerStatistics } from '../../shared/model/player-statistics.model';
import { PlayerProfileSpotlightStatistic } from './player-profile-spotlight/player-profile-spotlight-statistic.enum';
import { RegionService } from '@pages/hub/shared/services/region.service';
import { UserPreferenceService, UserPreferenceType } from '@shared/services/user-preferences.service';

@Component({
    selector: 'app-player-profile-statistics',
    templateUrl: './player-profile-statistics.component.html',
    styleUrls: ['./player-profile-statistics.component.scss']
})
export class PlayerProfileStatisticsComponent implements OnInit {

    playerId: number;
    playerStatistics: PlayerStatistics;
    regionId: number;
    isLoading = true;
    spotlightStatistic = PlayerProfileSpotlightStatistic;

    constructor(
        private route: ActivatedRoute,
        private playerService: PlayerService,
        private regionService: RegionService,
        private userPreferencesService: UserPreferenceService
    ) { }

    ngOnInit() {
        this.regionId = this.userPreferencesService.getUserPreference(UserPreferenceType.Region);
        this.route.parent.paramMap.pipe().subscribe(params => {
            this.playerId = +params.get('id');
            this.loadPlayerStatistics();
        });
    }

    loadPlayerStatistics() {
        this.isLoading = true;
        const filters: PlayerStatisticFilters = {
            playerId: this.playerId,
            timePeriod: TimePeriod.AllTime,
            includeSubstituteAppearances: true
        };
        this.regionService.getRegions().subscribe(regions => {
            const region = regions.find(r => r.regionId === this.regionId);
            filters.matchFormat = region.matchFormat;
            this.playerService.getPlayerStatistics(1, undefined, undefined, undefined, filters)
                .pipe(map(result => result.items[0])).subscribe(playerStatistics => {
                    this.playerStatistics = playerStatistics;
                    this.isLoading = false;
                });
        });
    }
}

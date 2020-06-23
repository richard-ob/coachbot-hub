import { Component, OnInit, Input } from '@angular/core';
import { Router } from '@angular/router';
import { PlayerService } from '@pages/hub/shared/services/player.service';
import { PlayerStatisticFilters } from '@pages/hub/shared/model/dtos/paged-player-statistics-request-dto.model';
import { PlayerStatistics } from '@pages/hub/shared/model/player-statistics.model';
import { TimePeriod } from '@pages/hub/shared/model/time-period.enum';
import { UserPreferenceService, UserPreferenceType } from '@shared/services/user-preferences.service';
import { PlayerSpotlightStatistic } from './player-spotlight-statistic.enum';

@Component({
    selector: 'app-player-spotlight',
    templateUrl: './player-spotlight.component.html',
    styleUrls: ['./player-spotlight.component.scss']
})
export class PlayerSpotlightComponent implements OnInit {

    @Input() statistic: PlayerSpotlightStatistic;
    filters = new PlayerStatisticFilters();
    spotlightPlayer: PlayerStatistics;
    apiModelProperty: string;
    modelProperty: string;
    measureName: string;
    heading: string;
    ordering: string;
    playerSpotlightStatistic = PlayerSpotlightStatistic;
    isLoading = true;

    constructor(
        private playerService: PlayerService,
        private userPreferencesService: UserPreferenceService,
        private router: Router
    ) {
    }

    ngOnInit() {
        this.setProperties(this.statistic);
        this.filters.regionId = this.userPreferencesService.getUserPreference(UserPreferenceType.Region);
        this.filters.timePeriod = TimePeriod.Year;
        this.filters.includeSubstituteAppearances = false;
        this.playerService.getPlayerStatistics(1, 1, this.apiModelProperty, this.ordering, this.filters).subscribe(playerStatistics => {
            if (playerStatistics.items.length > 0) {
                this.spotlightPlayer = playerStatistics.items[0];
            }
            this.isLoading = false;
        });
    }

    setProperties(playerSpotlightStatistic: PlayerSpotlightStatistic) {
        switch (playerSpotlightStatistic) {
            case PlayerSpotlightStatistic.Goals:
                this.modelProperty = 'goalsAverage';
                this.apiModelProperty = 'GoalsAverage';
                this.heading = 'Goal Scorer of the Week';
                this.measureName = 'Average Goals';
                this.ordering = 'DESC';
                break;
            case PlayerSpotlightStatistic.Assists:
                this.modelProperty = 'assistsAverage';
                this.apiModelProperty = 'AssistsAverage';
                this.heading = 'Assister of the Week';
                this.measureName = 'Average Assists';
                this.ordering = 'DESC';
                break;
            case PlayerSpotlightStatistic.GoalsConceded:
                this.modelProperty = 'goalsConcededAverage';
                this.apiModelProperty = 'GoalsConcededAverage';
                this.heading = 'Defender of the Week';
                this.measureName = 'Average Goals Conceded';
                this.ordering = 'ASC';
                break;
            case PlayerSpotlightStatistic.PassCompletion:
                this.modelProperty = 'passCompletionPercentageAverage';
                this.apiModelProperty = 'PassCompletionPercentageAverage';
                this.heading = 'Passer of the Week';
                this.measureName = 'Pass Completion';
                this.ordering = 'DESC';
                break;
        }
    }

    navigateToPlayerProfile() {
        this.router.navigate(['/player-profile', this.spotlightPlayer.playerId]);
    }

}

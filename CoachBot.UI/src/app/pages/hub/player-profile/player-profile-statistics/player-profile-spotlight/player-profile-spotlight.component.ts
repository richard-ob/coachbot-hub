import { Component, OnInit, Input } from '@angular/core';
import { TimePeriod } from '@pages/hub/shared/model/time-period.enum';
import { UserPreferenceService, UserPreferenceType } from '@shared/services/user-preferences.service';
import { Router } from '@angular/router';
import { PlayerProfileSpotlightStatistic } from './player-profile-spotlight-statistic.enum';
import { PlayerMatchStatistics } from '@pages/hub/shared/model/player-match-statistics.model';
import { PlayerService } from '@pages/hub/shared/services/player.service';
import { PlayerStatisticFilters } from '@pages/hub/shared/model/dtos/paged-player-statistics-request-dto.model';

@Component({
    selector: 'app-player-profile-spotlight',
    templateUrl: './player-profile-spotlight.component.html'
})
export class PlayerProfileSpotlightComponent implements OnInit {

    @Input() statistic: PlayerProfileSpotlightStatistic;
    @Input() playerId: number;
    filters = new PlayerStatisticFilters();
    spotlight: PlayerMatchStatistics;
    oppositionName: string;
    apiModelProperty: string;
    modelProperty: string;
    measureName: string;
    heading: string;
    ordering: string;
    iconClass: string;
    playerSpotlightStatistic = PlayerProfileSpotlightStatistic;
    isLoading = true;

    constructor(private playerService: PlayerService, private router: Router, private userPreferencesService: UserPreferenceService) { }

    ngOnInit() {
        this.setProperties(this.statistic);
        this.filters.regionId = this.userPreferencesService.getUserPreference(UserPreferenceType.Region);
        this.filters.timePeriod = TimePeriod.AllTime;
        this.filters.playerId = this.playerId;
        this.playerService.getPlayerMatchStatistics(1, 1, this.apiModelProperty, this.ordering, this.filters)
            .subscribe(playerStatistics => {
                console.log(playerStatistics)
                if (playerStatistics.items.length > 0) {
                    this.spotlight = playerStatistics.items[0];
                    this.setOppositionName();
                }
                this.isLoading = false;
            });
    }

    setProperties(teamSpotlightStatistic: PlayerProfileSpotlightStatistic) {
        switch (teamSpotlightStatistic) {
            case PlayerProfileSpotlightStatistic.MostGoals:
                this.modelProperty = 'goals';
                this.apiModelProperty = 'Goals';
                this.heading = 'Most Goals';
                this.measureName = 'Goals';
                this.ordering = 'DESC';
                this.iconClass = '';
                this.iconClass = 'icon-soccer-ball';
                break;
            case PlayerProfileSpotlightStatistic.MostAssists:
                this.modelProperty = 'assists';
                this.apiModelProperty = 'Assists';
                this.heading = 'Most Assists';
                this.measureName = 'Assists';
                this.ordering = 'DESC';
                this.iconClass = 'icon-keepers-glove';
                break;
            case PlayerProfileSpotlightStatistic.MostCompletedPasses:
                this.modelProperty = 'passesCompleted';
                this.apiModelProperty = 'passesCompleted';
                this.heading = 'Most Completed Passes';
                this.measureName = 'Passes';
                this.ordering = 'DESC';
                this.iconClass = 'icon-soccer-shots';
                break;
        }
    }

    setOppositionName(): void {
        this.oppositionName = this.spotlight.match.teamHomeId === this.spotlight.teamId ?
            this.spotlight.match.teamAway.name :
            this.spotlight.match.teamHome.name;
    }

    navigateToMatchOverview() {
        this.router.navigate(['/match-overview', this.spotlight.matchId]);
    }

}

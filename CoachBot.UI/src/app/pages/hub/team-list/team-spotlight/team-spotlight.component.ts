import { Component, OnInit, Input } from '@angular/core';
import { Router } from '@angular/router';
import { TimePeriod } from '@pages/hub/shared/model/time-period.enum';
import { UserPreferenceService, UserPreferenceType } from '@shared/services/user-preferences.service';
import { TeamSpotlightStatistic } from './team-spotlight-statistic.enum';
import { TeamStatistics } from '@pages/hub/shared/model/team-statistics.model';
import { TeamStatisticFilters } from '@pages/hub/shared/model/dtos/paged-team-statistics-request-dto.model';
import { TeamService } from '@pages/hub/shared/services/team.service';

@Component({
    selector: 'app-team-spotlight',
    templateUrl: './team-spotlight.component.html',
    styleUrls: ['./team-spotlight.component.scss']
})
export class TeamSpotlightComponent implements OnInit {

    @Input() statistic: TeamSpotlightStatistic;
    @Input() tournamentId: number;
    filters = new TeamStatisticFilters();
    spotlightTeam: TeamStatistics;
    apiModelProperty: string;
    modelProperty: string;
    measureName: string;
    heading: string;
    ordering: string;
    iconClass: string;
    teamSpotlightStatistic = TeamSpotlightStatistic;
    isLoading = true;

    constructor(
        private teamService: TeamService,
        private userPreferencesService: UserPreferenceService,
        private router: Router
    ) {
    }

    ngOnInit() {
        this.setProperties(this.statistic);
        this.filters.regionId = this.userPreferencesService.getUserPreference(UserPreferenceType.Region);
        if (this.tournamentId) {
            this.filters.tournamentId = this.tournamentId;
        } else {
            this.filters.timePeriod = TimePeriod.Week;
        }
        this.teamService.getTeamStatistics(1, 1, this.apiModelProperty, this.ordering, this.filters).subscribe(teamStatistics => {
            if (teamStatistics.items.length > 0) {
                this.spotlightTeam = teamStatistics.items[0];
            }
            this.isLoading = false;
        });
    }

    setProperties(teamSpotlightStatistic: TeamSpotlightStatistic) {
        switch (teamSpotlightStatistic) {
            case TeamSpotlightStatistic.Goals:
                this.modelProperty = 'goalsAverage';
                this.apiModelProperty = 'GoalsAverage';
                this.heading = $localize`:@@spotlight.goalScorersOfWeek:Goal Scorers of the Week`;
                this.measureName = $localize`:@@globals.averageGoals:Average Goals`;
                this.ordering = 'DESC';
                this.iconClass = '';
                this.iconClass = 'icon-soccer-ball';
                break;
            case TeamSpotlightStatistic.Wins:
                this.modelProperty = 'wins';
                this.apiModelProperty = 'Wins';
                this.heading = $localize`:@@spotlight.winnersOfWeek:Winners of the Week`;
                this.measureName = $localize`:@@globals.wins:Wins`;
                this.ordering = 'DESC';
                this.iconClass = 'icon-trophy';
                break;
            case TeamSpotlightStatistic.GoalsConceded:
                this.modelProperty = 'goalsConcededAverage';
                this.apiModelProperty = 'GoalsConcededAverage';
                this.heading = $localize`:@@spotlight.defensiveTeamOfWeek:Defensive Team of the Week`;
                this.measureName = $localize`:@@globals.averageGoalsConced:Average Goals Conceded`;
                this.ordering = 'ASC';
                this.iconClass = 'icon-keepers-glove';
                break;
            case TeamSpotlightStatistic.PassCompletion:
                this.modelProperty = 'passCompletionPercentageAverage';
                this.apiModelProperty = 'PassCompletionPercentageAverage';
                this.heading = $localize`:@@spotlight.passersOfWeek:Passers of the Week`;
                this.measureName = 'Pass Completion';
                this.ordering = 'DESC';
                this.iconClass = 'icon-soccer-shots';
                break;
        }
    }

    navigateToTeamProfile() {
        this.router.navigate(['/team-profile', this.spotlightTeam.teamId]);
    }

}

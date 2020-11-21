import { Component, OnInit, Input } from '@angular/core';
import { TimePeriod } from '@pages/hub/shared/model/time-period.enum';
import { UserPreferenceService, UserPreferenceType } from '@shared/services/user-preferences.service';
import { TeamStatisticFilters } from '@pages/hub/shared/model/dtos/paged-team-statistics-request-dto.model';
import { TeamService } from '@pages/hub/shared/services/team.service';
import { TeamProfileSpotlightStatistic } from './team-profile-spotlight-statistic.enum';
import { TeamMatchStatistics } from '@pages/hub/shared/model/team-match-statistics.model';
import { Router } from '@angular/router';
import { MatchOutcomeType } from '@pages/hub/shared/model/match-outcome-type.enum';
import { RegionService } from '@pages/hub/shared/services/region.service';

@Component({
    selector: 'app-team-profile-spotlight',
    templateUrl: './team-profile-spotlight.component.html'
})
export class TeamProfileSpotlightComponent implements OnInit {

    @Input() statistic: TeamProfileSpotlightStatistic;
    @Input() teamId: number;
    filters = new TeamStatisticFilters();
    spotlightTeam: TeamMatchStatistics;
    oppositionName: string;
    apiModelProperty: string;
    modelProperty: string;
    measureName: string;
    heading: string;
    ordering: string;
    iconClass: string;
    teamSpotlightStatistic = TeamProfileSpotlightStatistic;
    isLoading = true;

    constructor(
        private teamService: TeamService,
        private router: Router,
        private regionService: RegionService,
        private userPreferencesService: UserPreferenceService
    ) { }

    ngOnInit() {
        this.setProperties(this.statistic);
        this.filters.regionId = this.userPreferencesService.getUserPreference(UserPreferenceType.Region);
        this.filters.timePeriod = TimePeriod.AllTime;
        this.filters.teamId = this.teamId;
        this.regionService.getRegions().subscribe(regions => {
            const region = regions.find(r => r.regionId === this.filters.regionId);
            this.filters.matchFormat = region.matchFormat;
            this.teamService.getTeamMatchStatistics(1, 1, this.apiModelProperty, this.ordering, this.filters).subscribe(teamStatistics => {
                if (teamStatistics.items.length > 0) {
                    this.spotlightTeam = teamStatistics.items[0];
                    this.setOppositionName();
                }
                this.isLoading = false;
            });
        });
    }

    setProperties(teamSpotlightStatistic: TeamProfileSpotlightStatistic) {
        switch (teamSpotlightStatistic) {
            case TeamProfileSpotlightStatistic.BiggestWin:
                this.modelProperty = 'goals';
                this.apiModelProperty = 'Goals';
                this.heading = $localize`:@@spotlight.biggestWin:Biggest Win`;
                this.measureName = $localize`:@@globals.goalsScored:Goals Scored`;
                this.ordering = 'DESC';
                this.iconClass = '';
                this.iconClass = 'icon-soccer-ball';
                this.filters.matchOutcome = MatchOutcomeType.Win;
                break;
            case TeamProfileSpotlightStatistic.BiggestLoss:
                this.modelProperty = 'goalsConceded';
                this.apiModelProperty = 'GoalsConceded';
                this.heading = $localize`:@@spotlight.biggestLoss:Biggest Loss`;
                this.measureName = $localize`:@@globals.goalsConceded:Goals Conceded`;
                this.ordering = 'DESC';
                this.iconClass = 'icon-keepers-glove';
                this.filters.matchOutcome = MatchOutcomeType.Loss;
                break;
            case TeamProfileSpotlightStatistic.BestPassing:
                this.modelProperty = 'passesCompleted';
                this.apiModelProperty = 'passesCompleted';
                this.heading = $localize`:@@spotlight.mostCompletedPasses:Most Completed Passes`;
                this.measureName = $localize`:@@globals.passes:Passes`;
                this.ordering = 'DESC';
                this.iconClass = 'icon-soccer-shots';
                this.filters.matchOutcome = null;
                break;
        }
    }

    setOppositionName(): void {
        this.oppositionName = this.spotlightTeam.match.teamHomeId === this.teamId ?
            this.spotlightTeam.match.teamAway.name :
            this.spotlightTeam.match.teamHome.name;
    }

    navigateToMatchOverview() {
        this.router.navigate(['/match-overview', this.spotlightTeam.matchId]);
    }

}

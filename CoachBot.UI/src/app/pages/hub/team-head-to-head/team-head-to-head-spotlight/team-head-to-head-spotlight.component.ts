import { Component, OnInit, Input } from '@angular/core';
import { TimePeriod } from '@pages/hub/shared/model/time-period.enum';
import { TeamStatisticFilters } from '@pages/hub/shared/model/dtos/paged-team-statistics-request-dto.model';
import { TeamService } from '@pages/hub/shared/services/team.service';
import { TeamMatchStatistics } from '@pages/hub/shared/model/team-match-statistics.model';
import { Router } from '@angular/router';
import { MatchOutcomeType } from '@pages/hub/shared/model/match-outcome-type.enum';
import { TeamHeadToHeadSpotlightStatistic } from './team-head-to-head-spotlight-statistic.enum';
import { MatchTypes } from '@pages/hub/shared/model/match-types.enum';

@Component({
    selector: 'app-team-head-to-head-spotlight',
    templateUrl: './team-head-to-head-spotlight.component.html',
    styleUrls: ['./team-head-to-head-spotlight.component.scss']
})
export class TeamHeadToHeadSpotlightComponent implements OnInit {

    @Input() statistic: TeamHeadToHeadSpotlightStatistic;
    @Input() teamId: number;
    @Input() oppositionTeamId: number;
    @Input() matchType: MatchTypes;
    filters = new TeamStatisticFilters();
    spotlightTeam: TeamMatchStatistics;
    ribbonColour: string;
    apiModelProperty: string;
    modelProperty: string;
    measureName: string;
    heading: string;
    ordering: string;
    iconClass: string;
    teamSpotlightStatistic = TeamHeadToHeadSpotlightStatistic;    
    matchTypes = MatchTypes;
    isLoading = true;

    constructor(private teamService: TeamService, private router: Router) { }

    ngOnInit() {
        this.setProperties(this.statistic);
        this.filters.matchFormat = null;
        this.filters.matchType = this.matchType;
        this.filters.timePeriod = TimePeriod.AllTime;
        this.filters.teamId = this.teamId;
        this.filters.oppositionTeamId = this.oppositionTeamId;
        this.filters.headToHead = true;
        this.teamService.getTeamMatchStatistics(1, 1, this.apiModelProperty, this.ordering, this.filters).subscribe(teamStatistics => {
            this.spotlightTeam = teamStatistics.items[0];
            this.setRibbonColour();
            this.isLoading = false;
        });
    }

    setProperties(teamSpotlightStatistic: TeamHeadToHeadSpotlightStatistic) {
        switch (teamSpotlightStatistic) {
            case TeamHeadToHeadSpotlightStatistic.BiggestWin:
                this.modelProperty = 'goals';
                this.apiModelProperty = 'Goals';
                this.heading = $localize`:@@spotlight.biggestWin:Biggest Win`;
                this.measureName = $localize`:@@globals.goalsScored:Goals Scored`;
                this.ordering = 'DESC';
                this.iconClass = '';
                this.iconClass = 'icon-trophy';
                this.filters.matchOutcome = MatchOutcomeType.Win;
                break;
            case TeamHeadToHeadSpotlightStatistic.HighestPossession:
                this.modelProperty = 'possessionPercentage';
                this.apiModelProperty = 'PossessionPercentage';
                this.heading = $localize`:@@spotlight.mostPossession:Most Possession`;
                this.measureName = $localize`:@@globals.possessionPercentage:% Possession`;
                this.ordering = 'DESC';
                this.iconClass = 'icon-soccer-ball';
                this.filters.matchOutcome = null;
                break;
            case TeamHeadToHeadSpotlightStatistic.BestPassing:
                this.modelProperty = 'passesCompleted';
                this.apiModelProperty = 'PassesCompleted';
                this.heading = $localize`:@@spotlight.mostCompletedPasses:Most Completed Passes`;
                this.measureName = $localize`:@@globals.passes:Passes`;
                this.ordering = 'DESC';
                this.iconClass = 'icon-soccer-shots';
                this.filters.matchOutcome = null;
                break;
        }
    }
    
    setRibbonColour() {
        if (this.spotlightTeam.teamId === this.spotlightTeam.match.teamHomeId) {
            this.ribbonColour = this.spotlightTeam.match.teamHome.color;
        } else {
            this.ribbonColour = this.spotlightTeam.match.teamAway.color;
        }
    }

    navigateToMatchOverview() {
        this.router.navigate(['/match-overview', this.spotlightTeam.matchId]);
    }

}

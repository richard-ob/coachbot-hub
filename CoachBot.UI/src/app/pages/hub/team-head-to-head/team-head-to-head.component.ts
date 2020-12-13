import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { UserPreferenceService, UserPreferenceType } from '@shared/services/user-preferences.service';
import ColourUtils from '@shared/utilities/colour-utilities';
import { forkJoin } from 'rxjs';
import { StatisticType } from '../match-overview/model/statistic-type.enum';
import { DisplayValueMode } from '../shared/components/graphs/horizontal-bar-graph/horizontal-bar-graph.component';
import { TeamStatisticFilters } from '../shared/model/dtos/paged-team-statistics-request-dto.model';
import { MatchTypes } from '../shared/model/match-types.enum';
import { TeamStatistics } from '../shared/model/team-statistics.model';
import { Team } from '../shared/model/team.model';
import { TeamService } from '../shared/services/team.service';
import { TeamHeadToHeadSpotlightStatistic } from './team-head-to-head-spotlight/team-head-to-head-spotlight-statistic.enum';

@Component({
    selector: 'app-team-head-to-head',
    templateUrl: './team-head-to-head.component.html',
    styleUrls: ['./team-head-to-head.component.scss']
})
export class TeamHeadToHeadComponent implements OnInit {

    teamOne: Team;
    teamOneGradient: string;
    teamTwo: Team;
    teamTwoGradient: string;
    teamStatisticsOne: TeamStatistics;
    teamStatisticsTwo: TeamStatistics;
    matchType: MatchTypes = undefined;
    spotlightStatistic = TeamHeadToHeadSpotlightStatistic;
    statisticType = StatisticType;
    matchTypes = MatchTypes;
    displayValueModes = DisplayValueMode;
    isLoading = true;

    constructor(private teamService: TeamService, private route: ActivatedRoute, private userPreferenceService: UserPreferenceService) { }

    ngOnInit() {        
        const regionId = this.userPreferenceService.getUserPreference(UserPreferenceType.Region);
        this.route.params.subscribe(params => {
            forkJoin([
                this.teamService.getTeamByCode(params['teamOneCode'], regionId),
                this.teamService.getTeamByCode(params['teamTwoCode'], regionId)
            ]).subscribe(([teamOne, teamTwo]) => {
                this.teamOne = teamOne;
                this.teamTwo = teamTwo;
                this.teamOneGradient = this.generateGradient(this.teamOne.color);
                this.teamTwoGradient = this.generateGradient(this.teamTwo.color);
                this.setMatchType(undefined);
            });
        });
    }

    generateGradient(colour: string) {
        const gradientSrc =
            'linear-gradient(90deg,' + ColourUtils.hexToRgbA(colour, 0.6) + ',' + ColourUtils.hexToRgbA(colour, 0.3) + ')';
        return gradientSrc;
    }

    setMatchType(matchType: MatchTypes) {
        this.isLoading = true;
        const filtersTeamOne = new TeamStatisticFilters();
        const filtersTeamTwo = new TeamStatisticFilters();
        filtersTeamOne.teamId = this.teamOne.id;
        filtersTeamOne.oppositionTeamId = this.teamTwo.id;
        filtersTeamOne.matchFormat = undefined;
        filtersTeamOne.matchType = matchType;
        filtersTeamTwo.teamId = this.teamTwo.id;
        filtersTeamTwo.oppositionTeamId = this.teamOne.id;
        filtersTeamTwo.matchFormat = undefined;
        filtersTeamTwo.matchType = matchType;
        this.matchType = matchType;
        forkJoin([
            this.teamService.getTeamStatistics(1, undefined, undefined, undefined, filtersTeamOne),
            this.teamService.getTeamStatistics(1, undefined, undefined, undefined, filtersTeamTwo)
        ]).subscribe(([teamOneStatistics, teamTwoStatistics]) => {
            if (teamOneStatistics.items.length && teamTwoStatistics.items.length) {
                this.teamStatisticsOne = teamOneStatistics.items[0];
                this.teamStatisticsTwo = teamTwoStatistics.items[0];
            } else {
                this.teamStatisticsOne = null;
                this.teamStatisticsTwo = null;
            }
            this.isLoading = false;
        });
    }

}

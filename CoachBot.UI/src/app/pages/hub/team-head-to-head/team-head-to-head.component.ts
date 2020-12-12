import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import ColourUtils from '@shared/utilities/colour-utilities';
import { forkJoin } from 'rxjs';
import { StatisticType } from '../match-overview/model/statistic-type.enum';
import { DisplayValueMode } from '../shared/components/graphs/horizontal-bar-graph/horizontal-bar-graph.component';
import { TeamStatisticFilters } from '../shared/model/dtos/paged-team-statistics-request-dto.model';
import { TeamStatistics } from '../shared/model/team-statistics.model';
import { Team } from '../shared/model/team.model';
import { TeamService } from '../shared/services/team.service';

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
    statisticType = StatisticType;
    displayValueModes = DisplayValueMode;
    isLoading = true;

    constructor(private teamService: TeamService, private route: ActivatedRoute) { }

    ngOnInit() {
        this.route.params.subscribe(params => {
            forkJoin([
                this.teamService.getTeamByCode(params['teamOneCode'], 1),
                this.teamService.getTeamByCode(params['teamTwoCode'], 1)
            ]).subscribe(([teamOne, teamTwo]) => {
                this.teamOne = teamOne;
                this.teamTwo = teamTwo;
                this.teamOneGradient = this.generateGradient(this.teamOne.color);
                this.teamTwoGradient = this.generateGradient(this.teamTwo.color);
                const filtersTeamOne = new TeamStatisticFilters();
                const filtersTeamTwo = new TeamStatisticFilters();
                filtersTeamOne.teamId = teamOne.id;
                filtersTeamOne.oppositionTeamId = teamTwo.id;
                filtersTeamTwo.teamId = teamTwo.id;
                filtersTeamTwo.oppositionTeamId = teamOne.id;
                forkJoin([
                    this.teamService.getTeamStatistics(1, undefined, undefined, undefined, filtersTeamOne),
                    this.teamService.getTeamStatistics(1, undefined, undefined, undefined, filtersTeamTwo)
                ]).subscribe(([teamOneStatistics, teamTwoStatistics]) => {
                    if (teamOneStatistics.items.length && teamTwoStatistics.items.length) {
                        this.teamStatisticsOne = teamOneStatistics.items[0];
                        this.teamStatisticsTwo = teamTwoStatistics.items[0];
                    }
                    this.isLoading = false;
                });
            });
        });
    }

    generateGradient(colour: string) {
        const gradientSrc =
            'linear-gradient(90deg,' + ColourUtils.hexToRgbA(colour, 0.6) + ',' + ColourUtils.hexToRgbA(colour, 0.3) + ')';
        return gradientSrc;
    }

}

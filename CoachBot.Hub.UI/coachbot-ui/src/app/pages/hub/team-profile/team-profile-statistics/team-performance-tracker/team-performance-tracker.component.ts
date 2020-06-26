import { Component, OnInit, Input, ViewEncapsulation } from '@angular/core';
import { GraphSeries } from '@shared/models/graph-series.model';
import DateUtils from '@shared/utilities/date-utilities';
import { PerformanceTrackerTime } from '@pages/hub/shared/model/performance-tracker-time.enum';
import { PerformanceTrackerAttribute } from '@pages/hub/shared/model/performance-tracker-attribute.enum';
import { TeamService } from '@pages/hub/shared/services/team.service';
import { TeamPerformanceSnapshot } from '@pages/hub/shared/model/team-peformance-snapshot.model';

@Component({
    selector: 'app-team-performance-tracker',
    templateUrl: './team-performance-tracker.component.html',
    styleUrls: ['./team-performance-tracker.component.scss'],
    encapsulation: ViewEncapsulation.None
})
export class TeamPerformanceTrackerComponent implements OnInit {

    @Input() teamId: number;
    playerPerformanceSnapshots: TeamPerformanceSnapshot[];
    performanceSeries: GraphSeries[];
    currentPerformanceTrackerAttribute: PerformanceTrackerAttribute = PerformanceTrackerAttribute.MatchOutcomes;
    performanceTrackerAttribute = PerformanceTrackerAttribute;
    currentPerformanceTrackerTime: PerformanceTrackerTime = PerformanceTrackerTime.Daily;
    performanceTrackerTime = PerformanceTrackerTime;
    colorScheme = {
        domain: ['#28a745', '#5A5A5A', '#dc3545']
    };
    isLoading = true;

    constructor(private teamService: TeamService) {
    }

    ngOnInit() {
        this.getDailyPlayerPerformance();
    }

    getDailyPlayerPerformance() {
        this.isLoading = true;
        this.currentPerformanceTrackerTime = PerformanceTrackerTime.Daily;
        this.teamService.getDailyTeamPerformance(this.teamId).subscribe(playerPerformanceSnapshots => {
            this.playerPerformanceSnapshots = playerPerformanceSnapshots;
            this.mapPerformanceToPoints();
            this.isLoading = false;
        });
    }

    getWeeklyPlayerPerformance() {
        this.isLoading = true;
        this.currentPerformanceTrackerTime = PerformanceTrackerTime.Weekly;
        this.teamService.getWeeklyTeamPerformance(this.teamId).subscribe(playerPerformanceSnapshots => {
            this.playerPerformanceSnapshots = playerPerformanceSnapshots;
            this.isLoading = false;
        });
    }

    getMonthlyPlayerPerformance() {
        this.isLoading = true;
        this.currentPerformanceTrackerTime = PerformanceTrackerTime.Monthly;
        this.teamService.getMonthlyTeamPerformance(this.teamId).subscribe(playerPerformanceSnapshots => {
            this.playerPerformanceSnapshots = playerPerformanceSnapshots;
            this.mapPerformanceToPoints();
            this.isLoading = false;
        });
    }

    mapPerformanceToPoints() {
        let series: GraphSeries;
        switch (this.currentPerformanceTrackerAttribute) {
            case PerformanceTrackerAttribute.AverageGoals:
                series = this.generateSeries('Average Goals', 'averageGoals');
                this.performanceSeries = [series];
                this.setDefaultTheme();
                break;
            case PerformanceTrackerAttribute.AverageAssists:
                series = this.generateSeries('Average Assists', 'averageAssists');
                this.performanceSeries = [series];
                this.setDefaultTheme();
                break;
            case PerformanceTrackerAttribute.GoalsConceded:
                series = this.generateSeries('Average Goals Conceded', 'averageGoalsConceded');
                this.performanceSeries = [series];
                this.setDefaultTheme();
                break;
            case PerformanceTrackerAttribute.Cleansheets:
                series = this.generateSeries('Cleansheets', 'cleanSheets');
                this.performanceSeries = [series];
                this.setDefaultTheme();
                break;
            case PerformanceTrackerAttribute.Appearances:
                series = this.generateSeries('Appearances', 'appearances');
                this.performanceSeries = [series];
                this.setDefaultTheme();
                break;
            case PerformanceTrackerAttribute.MatchOutcomes:
                this.generateMatchOutcomeData();
                this.colorScheme = { domain: ['#28a745', '#5A5A5A', '#dc3545'] };
                break;
        }
    }

    setDefaultTheme() {
        this.colorScheme = { domain: ['#33b9f6'] };
    }

    generateSeries(name: string, property: string) {
        return {
            name,
            series: this.playerPerformanceSnapshots.map(
                snapshot => (
                    {
                        value: snapshot[property],
                        name: DateUtils.getDateString(snapshot.year, snapshot.month, snapshot.week, snapshot.day)
                    }
                )
            )
        };
    }

    generateMatchOutcomeData() {
        this.performanceSeries = [
            this.generateSeries('Wins', 'wins'), this.generateSeries('Draws', 'draws'), this.generateSeries('Losses', 'losses')
        ];
    }

    setAttribute(attribute: PerformanceTrackerAttribute) {
        this.currentPerformanceTrackerAttribute = attribute;
        this.mapPerformanceToPoints();
    }

}

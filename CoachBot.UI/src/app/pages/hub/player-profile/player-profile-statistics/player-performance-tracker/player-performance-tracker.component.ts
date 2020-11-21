import { Component, OnInit, Input, ViewEncapsulation } from '@angular/core';
import { PlayerService } from '../../../shared/services/player.service';
import { PlayerPerformanceSnapshot } from '@pages/hub/shared/model/player-performance-snapshot.model';
import { PerformanceTrackerTime } from '../../../shared/model/performance-tracker-time.enum';
import { PerformanceTrackerAttribute } from '../../../shared/model/performance-tracker-attribute.enum';
import { GraphSeries } from '@shared/models/graph-series.model';
import DateUtils from '@shared/utilities/date-utilities';

@Component({
    selector: 'app-player-performance-tracker',
    templateUrl: './player-performance-tracker.component.html',
    styleUrls: ['./player-performance-tracker.component.scss'],
    encapsulation: ViewEncapsulation.None
})
export class PlayerPerformanceTrackerComponent implements OnInit {

    @Input() playerId: number;
    playerPerformanceSnapshots: PlayerPerformanceSnapshot[];
    performanceSeries: GraphSeries[];
    currentPerformanceTrackerAttribute: PerformanceTrackerAttribute = PerformanceTrackerAttribute.MatchOutcomes;
    performanceTrackerAttribute = PerformanceTrackerAttribute;
    currentPerformanceTrackerTime: PerformanceTrackerTime = PerformanceTrackerTime.Continuous;
    performanceTrackerTime = PerformanceTrackerTime;
    colorScheme = {
        domain: ['#28a745', '#5A5A5A', '#dc3545']
    };
    showLegend = false;
    isLoading = true;

    constructor(private playerService: PlayerService) {
    }

    ngOnInit() {
        this.getContinuousPlayerPerformance();
    }

    getContinuousPlayerPerformance() {
        this.isLoading = true;
        this.currentPerformanceTrackerTime = PerformanceTrackerTime.Continuous;
        this.playerService.getContinuousPlayerPerformance(this.playerId).subscribe(playerPerformanceSnapshots => {
            this.playerPerformanceSnapshots = playerPerformanceSnapshots;
            this.mapPerformanceToPoints();
            this.isLoading = false;
        });
    }

    getDailyPlayerPerformance() {
        this.isLoading = true;
        this.currentPerformanceTrackerTime = PerformanceTrackerTime.Daily;
        this.playerService.getDailyPlayerPerformance(this.playerId).subscribe(playerPerformanceSnapshots => {
            this.playerPerformanceSnapshots = playerPerformanceSnapshots;
            this.mapPerformanceToPoints();
            this.isLoading = false;
        });
    }

    getWeeklyPlayerPerformance() {
        this.isLoading = true;
        this.currentPerformanceTrackerTime = PerformanceTrackerTime.Weekly;
        this.playerService.getWeeklyPlayerPerformance(this.playerId).subscribe(playerPerformanceSnapshots => {
            this.playerPerformanceSnapshots = playerPerformanceSnapshots;
            this.mapPerformanceToPoints();
            this.isLoading = false;
        });
    }

    getMonthlyPlayerPerformance() {
        this.isLoading = true;
        this.currentPerformanceTrackerTime = PerformanceTrackerTime.Monthly;
        this.playerService.getMonthlyPlayerPerformance(this.playerId).subscribe(playerPerformanceSnapshots => {
            this.playerPerformanceSnapshots = playerPerformanceSnapshots;
            this.mapPerformanceToPoints();
            this.isLoading = false;
        });
    }

    mapPerformanceToPoints() {
        let series: GraphSeries;
        switch (this.currentPerformanceTrackerAttribute) {
            case PerformanceTrackerAttribute.AverageGoals:
                series = this.generateSeries($localize`:@@globals.averageGoals:Average Goals`, 'averageGoals');
                this.performanceSeries = [series];
                this.showLegend = false;
                this.setDefaultTheme();
                break;
            case PerformanceTrackerAttribute.AverageAssists:
                series = this.generateSeries($localize`:@@globals.averageAssists:Average Assists`, 'averageAssists');
                this.performanceSeries = [series];
                this.showLegend = false;
                this.setDefaultTheme();
                break;
            case PerformanceTrackerAttribute.GoalsConceded:
                series = this.generateSeries($localize`:@@globals.averageGoalsConceded:Average Goals Conceded`, 'averageGoalsConceded');
                this.performanceSeries = [series];
                this.showLegend = false;
                this.setDefaultTheme();
                break;
            case PerformanceTrackerAttribute.Cleansheets:
                series = this.generateSeries($localize`:@@globals.cleansheets:Cleansheets`, 'cleanSheets');
                this.performanceSeries = [series];
                this.showLegend = false;
                this.setDefaultTheme();
                break;
            case PerformanceTrackerAttribute.Appearances:
                series = this.generateSeries($localize`:@@globals.appearances:Appearances`, 'appearances');
                this.performanceSeries = [series];
                this.showLegend = false;
                this.setDefaultTheme();
                break;
            case PerformanceTrackerAttribute.MatchOutcomes:
                this.generateMatchOutcomeData();
                this.showLegend = true;
                this.colorScheme = { domain: ['#44C424', '#5A5A5A', '#DD0000'] };
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
                        name: DateUtils.getDateString(snapshot.year, snapshot.month - 1, snapshot.week, snapshot.day)
                    }
                )
            )
        };
    }

    generateMatchOutcomeData() {
        this.performanceSeries = [
            this.generateSeries($localize`:@@globals.wins:Wins`, 'wins'),
            this.generateSeries($localize`:@@globals.draws:Draws`, 'draws'), 
            this.generateSeries($localize`:@@globals.losses:Losses`, 'losses')
        ];
    }

    setAttribute(attribute: PerformanceTrackerAttribute) {
        this.currentPerformanceTrackerAttribute = attribute;
        this.mapPerformanceToPoints();
    }

}

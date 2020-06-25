import { Component, OnInit, Input, ViewEncapsulation } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { PlayerService } from '../../../shared/services/player.service';
import { PlayerPerformanceSnapshot } from '@pages/hub/shared/model/player-performance-snapshot.model';
import { PerformanceTrackerTime } from './performance-tracker-time.enum';
import { GraphSeries } from './graph-series.model';
import { PerformanceTrackerAttribute } from './performance-tracker-attribute.enum';

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
    currentPerformanceTrackerAttribute: PerformanceTrackerAttribute = PerformanceTrackerAttribute.AverageGoals;
    performanceTrackerAttribute = PerformanceTrackerAttribute;
    currentPerformanceTrackerTime: PerformanceTrackerTime = PerformanceTrackerTime.Daily;
    performanceTrackerTime = PerformanceTrackerTime;
    isLoading = true;

    constructor(private route: ActivatedRoute, private playerService: PlayerService) {
    }

    ngOnInit() {
        this.getMonthlyPlayerPerformance();
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
                series = this.generateSeries('Average Goals', 'averageGoals');
                break;
            case PerformanceTrackerAttribute.AverageAssists:
                series = this.generateSeries('Average Assists', 'averageAssists');
                break;
            case PerformanceTrackerAttribute.GoalsConceded:
                series = this.generateSeries('Average Goals Conceded', 'averageGoalsConceded');
                break;
            case PerformanceTrackerAttribute.Cleansheets:
                series = this.generateSeries('Cleansheets', 'cleanSheets');
                break;
            case PerformanceTrackerAttribute.Appearances:
                series = this.generateSeries('Appearances', 'appearances');
                break;
        }
        this.performanceSeries = [series];
    }

    generateSeries(name: string, property: string) {
        return {
            name,
            series: this.playerPerformanceSnapshots.map(
                snapshot => ({ value: snapshot[property], name: this.getDateString(snapshot.year, snapshot.month) })
            )
        };
    }

    setAttribute(attribute: PerformanceTrackerAttribute) {
        this.currentPerformanceTrackerAttribute = attribute;
        this.mapPerformanceToPoints();
    }

    getDateString(year: number, month: number) {
        const date = new Date(year, month, 1);
        return `${date.toLocaleString('default', { month: 'long' })} ${year}`;
    }
}

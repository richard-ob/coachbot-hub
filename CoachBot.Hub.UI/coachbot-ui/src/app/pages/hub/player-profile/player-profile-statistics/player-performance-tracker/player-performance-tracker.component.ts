import { Component, OnInit, Input, ViewEncapsulation } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { PlayerService } from '../../../shared/services/player.service';
import { PlayerPerformanceSnapshot } from '@pages/hub/shared/model/player-performance-snapshot.model';
import { PerformanceTrackerTime } from './performance-tracker-time.enum';
import { GraphSeries } from './graph-series.model';
import { PerformanceTrackerAttribute } from './performance-tracker-attribute.enum';
import { DatePipe } from '@angular/common';

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
                snapshot => (
                    { value: snapshot[property], name: this.getDateString(snapshot.year, snapshot.month, snapshot.week, snapshot.day) })
            )
        };
    }

    setAttribute(attribute: PerformanceTrackerAttribute) {
        this.currentPerformanceTrackerAttribute = attribute;
        this.mapPerformanceToPoints();
    }

    getDateString(year: number, month: number, week: number, day: number) {
        const datePipe = new DatePipe('en-gb');

        if (day) {
            const date = new Date(year, month, day);
            return datePipe.transform(date, 'MMMM d');
        } else if (week) {
            const date = this.getDateOfWeek(week, year);
            return datePipe.transform(date, 'MMMM d');
        } else if (month) {
            const date = new Date(year, month, day || 1);
            return datePipe.transform(date, 'MMMM');
        } else {
            const date = new Date(year, 0, 1);
            return datePipe.transform(date, 'yyyy');
        }
    }

    getDateOfWeek(week: number, year: number) {
        const date = new Date(year, 0, (1 + (week - 1) * 7)); // Elle's method
        date.setDate(date.getDate() + (1 - date.getDay())); // 0 - Sunday, 1 - Monday etc
        return date;
    }
}

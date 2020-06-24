import { Component, OnInit, Input, ViewEncapsulation } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { PlayerService } from '../../../shared/services/player.service';
import { PlayerPerformanceSnapshot } from '@pages/hub/shared/model/player-performance-snapshot.model';
import { PerformanceTrackerTime } from './performance-tracker-time.enum';
import { GraphSeries } from './graph-series.model';

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
        this.playerService.getDailyPlayerPerformance(this.playerId).subscribe(playerPerformanceSnapshots => {
            this.playerPerformanceSnapshots = playerPerformanceSnapshots;
            this.mapPerformanceToPoints();
            this.isLoading = false;
        });
    }

    getWeeklyPlayerPerformance() {
        this.isLoading = true;
        this.playerService.getWeeklyPlayerPerformance(this.playerId).subscribe(playerPerformanceSnapshots => {
            this.playerPerformanceSnapshots = playerPerformanceSnapshots;
            this.mapPerformanceToPoints();
            this.isLoading = false;
        });
    }

    getMonthlyPlayerPerformance() {
        this.isLoading = true;
        this.playerService.getMonthlyPlayerPerformance(this.playerId).subscribe(playerPerformanceSnapshots => {
            this.playerPerformanceSnapshots = playerPerformanceSnapshots;
            this.mapPerformanceToPoints();
            this.isLoading = false;
        });
    }

    setFilters() {
        switch (this.currentPerformanceTrackerTime) {
            case PerformanceTrackerTime.Daily:
                this.getDailyPlayerPerformance();
                break;
            case PerformanceTrackerTime.Weekly:
                this.getWeeklyPlayerPerformance();
                break;
            case PerformanceTrackerTime.Monthly:
                this.getMonthlyPlayerPerformance();
                break;
        }
    }

    mapPerformanceToPoints() {
        this.performanceSeries = [
            {
                name: 'Goals',
                series: this.playerPerformanceSnapshots.map(snapshot => ({ value: snapshot.averageGoals, name: snapshot.month.toString() }))
            },
            {
                name: 'Assists',
                series: this.playerPerformanceSnapshots.map(snapshot => ({ value: snapshot.averageAssists, name: snapshot.month.toString() }))
            },
            {
                name: 'Goals Conceded',
                series: this.playerPerformanceSnapshots.map(snapshot => ({ value: snapshot.averageGoalsConceded, name: snapshot.month.toString() }))
            },
            {
                name: 'Cleansheets',
                series: this.playerPerformanceSnapshots.map(snapshot => ({ value: snapshot.cleanSheets, name: snapshot.month.toString() }))
            },
            {
                name: 'Appearances',
                series: this.playerPerformanceSnapshots.map(snapshot => ({ value: snapshot.appearances, name: snapshot.month.toString() }))
            }
        ];
    }
}

import { Component, OnInit, Input } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { PlayerService } from '../../../shared/services/player.service';
import { PlayerPerformanceSnapshot } from '@pages/hub/shared/model/player-performance-snapshot.model';
import { PerformanceTrackerTime } from './performance-tracker-time.enum';

@Component({
    selector: 'app-player-performance-tracker',
    templateUrl: './player-performance-tracker.component.html'
})
export class PlayerPerformanceTrackerComponent implements OnInit {

    @Input() playerId: number;
    playerPerformanceSnapshots: PlayerPerformanceSnapshot[];
    currentPerformanceTrackerTime: PerformanceTrackerTime = PerformanceTrackerTime.Daily;
    performanceTrackerTime = PerformanceTrackerTime;
    isLoading = true;

    constructor(private route: ActivatedRoute, private playerService: PlayerService) {
    }

    ngOnInit() {
        this.getDailyPlayerPerformance();
    }

    getDailyPlayerPerformance() {
        this.isLoading = true;
        this.playerService.getDailyPlayerPerformance(this.playerId).subscribe(playerPerformanceSnapshots => {
            this.playerPerformanceSnapshots = playerPerformanceSnapshots;
            this.isLoading = false;
        });
    }

    getWeeklyPlayerPerformance() {
        this.isLoading = true;
        this.playerService.getWeeklyPlayerPerformance(this.playerId).subscribe(playerPerformanceSnapshots => {
            this.playerPerformanceSnapshots = playerPerformanceSnapshots;
            this.isLoading = false;
        });
    }

    getMonthlyPlayerPerformance() {
        this.isLoading = true;
        this.playerService.getMonthlyPlayerPerformance(this.playerId).subscribe(playerPerformanceSnapshots => {
            this.playerPerformanceSnapshots = playerPerformanceSnapshots;
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
}

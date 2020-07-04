import { Component, OnInit, Input } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { OverviewType, CalendarHeatmapData } from 'angular2-calendar-heatmap';
import { PlayerService } from '../../../shared/services/player.service';
import { MatchDayTotals } from '../../../shared/model/team-match-day-totals';

const monthName = new Intl.DateTimeFormat('en-us', { month: 'short' });
const weekdayName = new Intl.DateTimeFormat('en-us', { weekday: 'short' });

@Component({
    selector: 'app-player-profile-activity-heatmap',
    templateUrl: './player-profile-activity-heatmap.component.html',
    styleUrls: ['./player-profile-activity-heatmap.component.scss']
})
export class PlayerProfileActivityHeatmapComponent implements OnInit {

    @Input() playerId: number;
    isLoading = true;
    appearanceTotals: MatchDayTotals[];

    constructor(private route: ActivatedRoute, private playerService: PlayerService) { }

    ngOnInit() {
        this.route.parent.paramMap.pipe().subscribe(params => {
            this.playerId = +params.get('id');
            this.loadPlayerAppearances();
        });
    }

    private loadPlayerAppearances() {
        this.isLoading = true;
        this.playerService.getPlayerAppearanceTotals(this.playerId).subscribe(appearanceTotals => {
            this.appearanceTotals = appearanceTotals;
            this.isLoading = false;
        });
    }

    private getAppearanceTotalForDate(totalsForDate: Date) {
        const appearanceTotal = this.appearanceTotals.find(t => new Date(t.matchDayDate).getTime() === totalsForDate.getTime());

        if (appearanceTotal) {
            return appearanceTotal.matches;
        }

        return 1;
    }
}

import { Component, OnInit, Input } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { OverviewType, CalendarHeatmapData } from 'angular2-calendar-heatmap';
import { PlayerService } from '../../../shared/services/player.service';
import { MatchDayTotals } from '../../../shared/model/team-match-day-totals';

@Component({
    selector: 'app-player-profile-activity-heatmap',
    templateUrl: './player-profile-activity-heatmap.component.html',
    styleUrls: ['./player-profile-activity-heatmap.component.scss']
})
export class PlayerProfileActivityHeatmapComponent implements OnInit {

    @Input() playerId: number;
    isLoading = true;
    overview = OverviewType.year;
    appearanceTotals: MatchDayTotals[];
    appearanceData: CalendarHeatmapData[];

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
            this.generateAppearanceData();
            this.isLoading = false;
        });
    }

    private generateAppearanceData() {
        this.appearanceData = [];
        const startDate = new Date(new Date().getFullYear(), 0, 1);
        //const now = new Date();        
        const now = new Date(new Date().getFullYear() + 1, 0, 1);
        for (const day = startDate; day <= now; day.setDate(day.getDate() + 1)) {
            const dailyAppearance = {
                date: new Date(day),
                total: this.getAppearanceTotalForDate(day),
                details: []
            };
            this.appearanceData.push(dailyAppearance);
        }
    }

    private getAppearanceTotalForDate(totalsForDate: Date) {
        const appearanceTotal = this.appearanceTotals.find(t => new Date(t.matchDayDate).getTime() === totalsForDate.getTime());

        if (appearanceTotal) {
            return appearanceTotal.matches;
        }

        return 1;
    }

    doLog() {
        console.log('hit it');
    }
}

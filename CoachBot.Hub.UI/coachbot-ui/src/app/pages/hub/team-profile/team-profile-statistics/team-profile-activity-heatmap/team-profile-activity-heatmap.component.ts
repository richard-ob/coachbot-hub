import { Component, OnInit, Input } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { OverviewType, CalendarHeatmapData } from 'angular2-calendar-heatmap';
import { MatchDayTotals } from '../../../shared/model/team-match-day-totals';
import { TeamService } from '../../../shared/services/team.service';

@Component({
    selector: 'app-team-profile-activity-heatmap',
    templateUrl: './team-profile-activity-heatmap.component.html',
    styleUrls: ['./team-profile-activity-heatmap.component.scss']
})
export class TeamProfileActivityHeatmapComponent implements OnInit {

    @Input() teamId: number;
    isLoading = true;
    overview = OverviewType.year;
    teamMatchTotals: MatchDayTotals[];
    matchData: CalendarHeatmapData[];

    constructor(private route: ActivatedRoute, private teamService: TeamService) { }

    ngOnInit() {
        this.loadTeamMatchTotal();
    }

    private loadTeamMatchTotal() {
        this.isLoading = true;
        this.teamService.getTeamMatchDayTotals(this.teamId).subscribe(teamMatchTotals => {
            this.teamMatchTotals = teamMatchTotals;
            this.generateMatchData();
            this.isLoading = false;
        });
    }

    private generateMatchData() {
        this.matchData = [];
        const startDate = new Date(new Date().getFullYear(), 0, 1);
        //const now = new Date();        
        const now = new Date(new Date().getFullYear() + 1, 0, 1);
        for (const day = startDate; day <= now; day.setDate(day.getDate() + 1)) {
            const dailyAppearance = {
                date: new Date(day),
                total: this.getAppearanceTotalForDate(day),
                details: []
            };
            this.matchData.push(dailyAppearance);
        }
    }

    private getAppearanceTotalForDate(totalsForDate: Date) {
        const teamMatchTotal = this.teamMatchTotals.find(t => new Date(t.matchDayDate).getTime() === totalsForDate.getTime());

        if (teamMatchTotal) {
            return teamMatchTotal.matches;
        }

        return 1;
    }
}

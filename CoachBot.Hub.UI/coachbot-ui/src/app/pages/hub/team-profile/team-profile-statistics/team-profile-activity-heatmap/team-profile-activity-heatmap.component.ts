import { Component, OnInit, Input } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
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
    teamMatchTotals: MatchDayTotals[];

    constructor(private route: ActivatedRoute, private teamService: TeamService) { }

    ngOnInit() {
        this.loadTeamMatchTotal();
    }

    private loadTeamMatchTotal() {
        this.isLoading = true;
        this.teamService.getTeamMatchDayTotals(this.teamId).subscribe(teamMatchTotals => {
            this.teamMatchTotals = teamMatchTotals;
            this.isLoading = false;
        });
    }
}

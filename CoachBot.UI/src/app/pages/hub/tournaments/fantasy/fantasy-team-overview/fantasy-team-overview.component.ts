import { Component, OnInit } from '@angular/core';
import { FantasyService } from '../../../shared/services/fantasy.service';
import { FantasyTeam } from '../../../shared/model/fantasy-team.model';
import { ActivatedRoute } from '@angular/router';
import { PositionGroup } from '../../../shared/model/position-group.enum';
import { Tournament } from '@pages/hub/shared/model/tournament.model';
import { SwalPortalTargets } from '@sweetalert2/ngx-sweetalert2';
import { FantasyPlayerPerformance } from '@pages/hub/shared/model/fantasy-player-performance.model';
import { FantasyTeamRank } from '@pages/hub/shared/model/fantasy-team-rank.model';

@Component({
    selector: 'app-fantasy-team-overview',
    templateUrl: './fantasy-team-overview.component.html',
    styleUrls: ['./fantasy-team-overview.component.scss']
})
export class FantasyTeamOverviewComponent implements OnInit {

    fantasyTeamId: number;
    fantasyTeam: FantasyTeam;
    fantasyPlayerPerformances: FantasyPlayerPerformance[];
    fantasyTeamRanking: FantasyTeamRank;
    tournament: Tournament;
    positionGroups = PositionGroup;
    isLoading = true;
    isUpdating = false;

    constructor(
        private fantasyService: FantasyService,
        private route: ActivatedRoute,
        public readonly swalTargets: SwalPortalTargets
    ) { }

    ngOnInit() {
        this.route.paramMap.pipe().subscribe(params => {
            this.fantasyTeamId = +params.get('id');
            this.fantasyService.getFantasyTeam(this.fantasyTeamId).subscribe(fantasyTeam => {
                this.fantasyTeam = fantasyTeam;
                this.fantasyService.getFantasyPlayerPeformances(this.fantasyTeamId).subscribe(playerPerformances => {
                    this.fantasyPlayerPerformances = playerPerformances;
                    this.fantasyService.getFantasyTeamRankings(this.fantasyTeam.tournamentId).subscribe(rankings => {
                        if (rankings.some(r => r.fantasyTeamId === this.fantasyTeamId)) {
                            this.fantasyTeamRanking = rankings.find(r => r.fantasyTeamId === this.fantasyTeamId);
                        }
                        this.isLoading = false;
                    });
                });
            });
        });
    }
}

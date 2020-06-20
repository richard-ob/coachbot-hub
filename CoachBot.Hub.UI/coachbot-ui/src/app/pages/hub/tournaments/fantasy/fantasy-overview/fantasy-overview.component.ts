import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FantasyService } from '@pages/hub/shared/services/fantasy.service';
import { FantasyTeamRank } from '@pages/hub/shared/model/fantasy-team-rank.model';

@Component({
    selector: 'app-fantasy-overview',
    templateUrl: './fantasy-overview.component.html'
})
export class FantasyOverviewComponent implements OnInit {

    fantasyTeamRankings: FantasyTeamRank[];
    isCreating = false;
    isLoading = true;

    constructor(private fantasyService: FantasyService, private route: ActivatedRoute, private router: Router) { }

    ngOnInit() {
        this.route.paramMap.pipe().subscribe(params => {
            const tournamentId = +params.get('tournamentId');
            this.fantasyService.getFantasyTeamRankings(tournamentId).subscribe(rankings => {
                this.fantasyTeamRankings = rankings;
                this.isLoading = false;
            });
        });
    }


}

import { Component, OnInit, Input, ViewEncapsulation } from '@angular/core';
import { FantasyService } from '@pages/hub/shared/services/fantasy.service';
import { FantasyTeamRank } from '@pages/hub/shared/model/fantasy-team-rank.model';
import { Router } from '@angular/router';

@Component({
    selector: 'app-fantasy-team-spotlight',
    templateUrl: './fantasy-team-spotlight.component.html'
})
export class FantasyTeamSpotlightComponent implements OnInit {

    @Input() tournamentId: number;
    spotlightTeam: FantasyTeamRank;
    isCreating = false;
    isLoading = true;

    constructor(
        private fantasyService: FantasyService,
        private router: Router
    ) { }

    ngOnInit() {
        this.fantasyService.getFantasyTeamSpotlight(this.tournamentId).subscribe(spotlightTeam => {
            this.spotlightTeam = spotlightTeam;
            this.isLoading = false;
        });
    }

    navigateToFantasyTeam(fantasyTeamId: number) {
        this.router.navigate(['/fantasy-overview/', fantasyTeamId]);
    }

}

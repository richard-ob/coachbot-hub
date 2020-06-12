import { Component, OnInit } from '@angular/core';
import { FantasyService } from '../../shared/services/fantasy.service';
import { FantasyTeam } from '../../shared/model/fantasy-team.model';
import { Router } from '@angular/router';
import { Tournament } from '@pages/hub/shared/model/tournament.model';

@Component({
    selector: 'app-fantasy-team-manager',
    templateUrl: './fantasy-team-manager.component.html'
})
export class FantasyTeamManagerComponent implements OnInit {

    availableTournament: Tournament[];
    fantasyTeams: FantasyTeam[];
    fantasyTeam: FantasyTeam = new FantasyTeam();
    isCreating = false;
    isLoading = true;

    constructor(private fantasyService: FantasyService, private router: Router) { }

    ngOnInit() {
        // TODO: Get for user
        this.fantasyService.getFantasyTeams(6).subscribe(fantasyTeams => {
            this.fantasyTeams = fantasyTeams;
            this.fantasyService.getAvailableFantasyTournamentsForUser().subscribe(availableTournament => {
                this.availableTournament = availableTournament;
                this.isLoading = false;
            });
        });
    }

    createFantasyTeam() {
        this.isCreating = true;
        this.fantasyTeam.playerId = 1;
        this.fantasyService.createFantasyTeam(this.fantasyTeam).subscribe(() => {
            this.isCreating = false;
            this.fantasyTeam = new FantasyTeam();
        });
    }

    navigateToFantasyTeam(fantasyTeamId: number) {
        this.router.navigate(['/fantasy-editor', fantasyTeamId]);
    }

}

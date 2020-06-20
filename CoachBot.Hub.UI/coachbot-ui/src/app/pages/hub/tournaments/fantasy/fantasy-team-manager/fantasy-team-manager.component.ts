import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Tournament } from '@pages/hub/shared/model/tournament.model';
import { FantasyService } from '@pages/hub/shared/services/fantasy.service';
import { FantasyTeam } from '@pages/hub/shared/model/fantasy-team.model';

@Component({
    selector: 'app-fantasy-team-manager',
    templateUrl: './fantasy-team-manager.component.html'
})
export class FantasyTeamManagerComponent implements OnInit {

    availableTournaments: Tournament[];
    fantasyTeams: FantasyTeam[];
    fantasyTeam: FantasyTeam = new FantasyTeam();
    isCreating = false;
    isLoading = true;

    constructor(private fantasyService: FantasyService, private router: Router) { }

    ngOnInit() {
        this.fantasyService.getFantasyTeamsForUser().subscribe(fantasyTeams => {
            this.fantasyTeams = fantasyTeams;
            this.fantasyService.getAvailableFantasyTournamentsForUser().subscribe(availableTournaments => {
                this.availableTournaments = availableTournaments;
                this.isLoading = false;
            });
        });
    }

    createFantasyTeam() {
        this.isCreating = true;
        this.fantasyService.createFantasyTeam(this.fantasyTeam).subscribe(() => {
            this.isCreating = false;
            this.fantasyTeam = new FantasyTeam();
        });
    }

    navigateToFantasyTeam(fantasyTeamId: number) {
        this.router.navigate(['/fantasy-editor', fantasyTeamId]);
    }

}

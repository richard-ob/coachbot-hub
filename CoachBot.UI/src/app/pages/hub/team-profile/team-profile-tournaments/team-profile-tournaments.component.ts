import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { TournamentService } from '@pages/hub/shared/services/tournament.service';
import { Tournament } from '@pages/hub/shared/model/tournament.model';

@Component({
    selector: 'app-team-profile-tournaments',
    templateUrl: './team-profile-tournaments.component.html'
})
export class TeamProfileTournamentsComponent implements OnInit {

    teamId: number;
    tournaments: Tournament[];
    isLoading = true;

    constructor(private router: Router, private route: ActivatedRoute, private tournamentService: TournamentService) { }

    ngOnInit() {
        this.route.parent.paramMap.pipe().subscribe(params => {
            this.teamId = +params.get('id');
            this.tournamentService.getTournamentsForTeam(this.teamId).subscribe(tournaments => {
                this.tournaments = tournaments;
                this.isLoading = false;
            });
        });
    }

    navigateToTournament(tournamentId: number) {
        this.router.navigate(['/tournament', tournamentId]);
    }

}

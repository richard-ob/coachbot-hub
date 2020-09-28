import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Tournament } from '@pages/hub/shared/model/tournament.model';
import { TournamentService } from '@pages/hub/shared/services/tournament.service';

@Component({
    selector: 'app-player-profile-tournaments',
    templateUrl: './player-profile-tournaments.component.html'
})
export class PlayerProfileTournamentsComponent implements OnInit {

    isLoading = true;
    tournaments: Tournament[];

    constructor(private router: Router, private route: ActivatedRoute, private tournamentService: TournamentService) { }

    ngOnInit() {
        this.route.parent.paramMap.pipe().subscribe(params => {
            const playerId = +params.get('id');
            this.tournamentService.getTournamentsForPlayer(playerId).subscribe(tournaments => {
                this.tournaments = tournaments;
                this.isLoading = false;
            });
        });
    }

    navigateToTournament(tournamentId: number) {
        this.router.navigate(['/tournament', tournamentId]);
    }

}

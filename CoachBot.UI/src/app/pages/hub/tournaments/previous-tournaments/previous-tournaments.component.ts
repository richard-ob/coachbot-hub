import { Component, OnInit } from '@angular/core';
import { TournamentService } from '../../shared/services/tournament.service';
import { Router } from '@angular/router';
import { Tournament } from '@pages/hub/shared/model/tournament.model';

@Component({
    selector: 'app-previous-tournaments',
    templateUrl: './previous-tournaments.component.html'
})
export class PreviousTournamentsComponent implements OnInit {

    tournaments: Tournament[];
    isLoading = true;

    constructor(private tournamentService: TournamentService, private router: Router) { }

    ngOnInit() {
        this.tournamentService.getPastTournaments().subscribe(tournaments => {
            this.tournaments = tournaments;
            this.isLoading = false;
        });
    }

    navigateToTournament(tournamentId: number) {
        this.router.navigate(['/tournament', tournamentId]);
    }
}

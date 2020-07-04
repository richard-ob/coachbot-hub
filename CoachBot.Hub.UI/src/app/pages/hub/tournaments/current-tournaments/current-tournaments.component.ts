import { Component, OnInit } from '@angular/core';
import { TournamentService } from '../../shared/services/tournament.service';
import { Router } from '@angular/router';
import { Tournament } from '@pages/hub/shared/model/tournament.model';

@Component({
    selector: 'app-current-tournaments',
    templateUrl: './current-tournaments.component.html'
})
export class CurrentTournamentsComponent implements OnInit {

    tournaments: Tournament[];
    isLoading = true;

    constructor(private tournamentService: TournamentService, private router: Router) { }

    ngOnInit() {
        this.tournamentService.getCurrentTournaments().subscribe(tournaments => {
            this.tournaments = tournaments;
        });
    }

    navigateToTournament(tournamentId: number) {
        this.router.navigate(['/tournament', tournamentId]);
    }
}

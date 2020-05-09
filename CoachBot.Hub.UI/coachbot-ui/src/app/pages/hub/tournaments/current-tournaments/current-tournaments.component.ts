import { Component, OnInit } from '@angular/core';
import { TournamentService } from '../../shared/services/tournament.service';
import { TournamentEdition } from '../../shared/model/tournament-edition.model';
import { Router } from '@angular/router';

@Component({
    selector: 'app-current-tournaments',
    templateUrl: './current-tournaments.component.html'
})
export class CurrentTournamentsComponent implements OnInit {

    tournamentEditions: TournamentEdition[];
    isLoading = true;

    constructor(private tournamentService: TournamentService, private router: Router) { }

    ngOnInit() {
        this.tournamentService.getTournamentEditions().subscribe(tournamentEditions => {
            this.tournamentEditions = tournamentEditions;
        });
    }

    navigateToTournamentEdition(tournamentEditionId: number) {
        this.router.navigate(['/tournament', tournamentEditionId]);
    }
}

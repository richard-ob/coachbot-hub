import { Component, OnInit } from '@angular/core';
import { Tournament } from '../../shared/tournament.model';
import { TournamentService } from '../../shared/services/tournament.service';

@Component({
    selector: 'app-tournament-creator',
    templateUrl: './tournament-creator.component.html'
})
export class TournamentCreatorComponent implements OnInit {

    tournament: Tournament = new Tournament();
    tournaments: Tournament[];
    isCreating = false;
    isLoading = true;

    constructor(private tournamentService: TournamentService) { }

    ngOnInit() {
        this.loadTournaments();
    }

    createTournament() {
        this.isCreating = true;
        this.tournamentService.createTournament(this.tournament).subscribe(() => {
            this.isCreating = false;
            this.loadTournaments();
        });
    }

    loadTournaments() {
        this.isLoading = true;
        this.tournamentService.getTournaments().subscribe(tournaments => {
            this.tournaments = tournaments;
            this.isLoading = false;
        });
    }

}

import { Component, OnInit } from '@angular/core';
import { TournamentService } from '../../shared/services/tournament.service';
import { TournamentSeries } from '@pages/hub/shared/model/tournament-series.model';

@Component({
    selector: 'app-tournament-creator',
    templateUrl: './tournament-creator.component.html'
})
export class TournamentCreatorComponent implements OnInit {

    tournamentSeries: TournamentSeries = new TournamentSeries();
    existingTournamentSeries: TournamentSeries[];
    isCreating = false;
    isLoading = true;

    constructor(private tournamentService: TournamentService) { }

    ngOnInit() {
        this.loadTournaments();
    }

    createTournament() {
        this.isCreating = true;
        this.tournamentService.createTournamentSeries(this.tournamentSeries).subscribe(() => {
            this.isCreating = false;
            this.tournamentSeries = new TournamentSeries();
            this.loadTournaments();
        });
    }

    loadTournaments() {
        this.isLoading = true;
        this.tournamentService.getTournamentSeries().subscribe(existingTournamentSeries => {
            this.existingTournamentSeries = existingTournamentSeries;
            this.isLoading = false;
        });
    }

}

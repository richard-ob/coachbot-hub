import { Component, OnInit } from '@angular/core';
import { Tournament } from '../../shared/tournament.model';
import { TournamentService } from '../../shared/services/tournament.service';
import { TournamentEdition } from '../../shared/model/tournament-edition.model';
import { ActivatedRoute } from '@angular/router';

@Component({
    selector: 'app-tournament-series-editor',
    templateUrl: './tournament-series-editor.component.html'
})
export class TournamentSeriesEditorComponent implements OnInit {

    tournamentId: number;
    tournamentEdition: TournamentEdition = new TournamentEdition();
    tournament: Tournament;
    isCreating = false;
    isLoading = true;

    constructor(private tournamentService: TournamentService, private route: ActivatedRoute) { }

    ngOnInit() {
        this.route.paramMap.pipe().subscribe(params => {
            this.tournamentId = +params.get('id');
            this.loadTournament();
        });
    }

    createTournamentEdition() {
        this.isCreating = true;
        this.tournamentEdition.tournamentId = this.tournamentId;
        this.tournamentService.createTournamentEdition(this.tournamentEdition).subscribe(() => {
            this.isCreating = false;
            this.loadTournament();
            this.tournamentEdition = new TournamentEdition();
        });
    }

    loadTournament() {
        this.isLoading = true;
        this.tournamentService.getTournament(this.tournamentId).subscribe(tournament => {
            this.tournament = tournament;
            this.isLoading = false;
        });
    }

}

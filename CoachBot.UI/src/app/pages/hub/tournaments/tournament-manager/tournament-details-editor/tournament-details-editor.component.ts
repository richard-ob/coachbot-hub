import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { TournamentService } from '@pages/hub/shared/services/tournament.service';
import { Tournament } from '@pages/hub/shared/model/tournament.model';

@Component({
    selector: 'app-tournament-details-editor',
    templateUrl: './tournament-details-editor.component.html'
})
export class TournamentDetailsEditorComponent implements OnInit {

    tournamentId: number;
    tournament: Tournament;
    isSaving = false;
    isLoading = true;

    constructor(
        private tournamentService: TournamentService,
        private route: ActivatedRoute,
        private router: Router
    ) { }

    ngOnInit() {
        this.route.parent.paramMap.pipe().subscribe(params => {
            this.tournamentId = +params.get('id');
            this.loadTournament();
        });
    }

    loadTournament() {
        this.isLoading = true;
        this.tournamentService.getTournament(this.tournamentId).subscribe(tournament => {
            this.tournament = tournament;
            this.isLoading = false;
        });
    }

    updateTournament() {
        this.isLoading = true;
        this.tournamentService.updateTournament(this.tournament).subscribe(() => {
            this.loadTournament();
        });
    }


}

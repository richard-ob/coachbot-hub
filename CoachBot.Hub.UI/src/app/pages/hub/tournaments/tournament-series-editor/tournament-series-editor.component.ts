import { Component, OnInit } from '@angular/core';
import { TournamentService } from '../../shared/services/tournament.service';
import { Tournament } from '../../shared/model/tournament.model';
import { ActivatedRoute } from '@angular/router';
import { TournamentSeries } from '@pages/hub/shared/model/tournament-series.model';
import { TeamType } from '@pages/hub/shared/model/team-type.enum';

@Component({
    selector: 'app-tournament-series-editor',
    templateUrl: './tournament-series-editor.component.html'
})
export class TournamentSeriesEditorComponent implements OnInit {

    tournamentSeriesId: number;
    tournament: Tournament = new Tournament();
    tournamentSeries: TournamentSeries;
    teamTypes = TeamType;
    isCreating = false;
    isLoading = true;

    constructor(private tournamentService: TournamentService, private route: ActivatedRoute) { }

    ngOnInit() {
        this.route.paramMap.pipe().subscribe(params => {
            this.tournamentSeriesId = +params.get('id');
            this.loadTournamentSeries();
        });
    }

    createTournament() {
        this.isCreating = true;
        this.tournament.tournamentSeriesId = this.tournamentSeriesId;
        this.tournamentService.createTournament(this.tournament).subscribe(() => {
            this.isCreating = false;
            this.loadTournamentSeries();
            this.tournament = new Tournament();
        });
    }

    loadTournamentSeries() {
        this.isLoading = true;
        this.tournamentService.getTournamentSeriesById(this.tournamentSeriesId).subscribe(tournamentSeries => {
            this.tournamentSeries = tournamentSeries;
            this.isLoading = false;
        });
    }

}

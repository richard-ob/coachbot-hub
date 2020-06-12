import { Component, OnInit } from '@angular/core';
import { TournamentService } from '../../shared/services/tournament.service';
import { Tournament } from '../../shared/model/tournament.model';
import { ActivatedRoute } from '@angular/router';

@Component({
    selector: 'app-tournament-overview',
    templateUrl: './tournament-overview.component.html'
})
export class TournamentOverviewComponent implements OnInit {

    tournamentId: number;
    tournament: Tournament;
    isLoading = true;

    constructor(private tournamentService: TournamentService, private route: ActivatedRoute) { }

    ngOnInit() {
        this.route.paramMap.pipe().subscribe(params => {
            this.tournamentId = +params.get('id');
            this.tournamentService.getTournament(this.tournamentId).subscribe(tournament => {
                this.tournament = tournament;
                this.isLoading = false;
            });
        });
    }

}

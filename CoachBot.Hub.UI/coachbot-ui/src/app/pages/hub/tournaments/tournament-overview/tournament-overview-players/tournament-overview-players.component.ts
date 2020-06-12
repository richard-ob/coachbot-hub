import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { FantasyTeam } from '../../../shared/model/fantasy-team.model';

@Component({
    selector: 'app-tournament-overview-players',
    templateUrl: './tournament-overview-players.component.html'
})
export class TournamentOverviewPlayersComponent implements OnInit {

    tournamentId: number;
    isLoading = true;

    constructor(private route: ActivatedRoute) { }

    ngOnInit() {
        this.route.parent.paramMap.pipe().subscribe(params => {
            this.tournamentId = +params.get('id');
        });
    }
}

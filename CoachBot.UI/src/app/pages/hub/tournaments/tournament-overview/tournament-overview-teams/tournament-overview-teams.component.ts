import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
    selector: 'app-tournament-overview-teams',
    templateUrl: './tournament-overview-teams.component.html'
})
export class TournamentOverviewTeamsComponent implements OnInit {

    tournamentId: number;
    isLoading = true;

    constructor(private route: ActivatedRoute) { }

    ngOnInit() {
        this.route.parent.paramMap.pipe().subscribe(params => {
            this.tournamentId = +params.get('id');
        });
    }

}

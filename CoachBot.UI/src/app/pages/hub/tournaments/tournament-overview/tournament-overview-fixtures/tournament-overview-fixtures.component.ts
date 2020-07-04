import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
    selector: 'app-tournament-overview-fixtures',
    templateUrl: './tournament-overview-fixtures.component.html'
})
export class TournamentOverviewFixturesComponent implements OnInit {

    tournamentId: number;
    isLoading = true;

    constructor(private route: ActivatedRoute) { }

    ngOnInit() {
        this.route.parent.paramMap.pipe().subscribe(params => {
            this.tournamentId = +params.get('id');
        });
    }

}

import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
    selector: 'app-tournament-overview-fixtures',
    templateUrl: './tournament-overview-fixtures.component.html'
})
export class TournamentOverviewFixturesComponent implements OnInit {

    tournamentEditionId: number;
    isLoading = true;

    constructor(private route: ActivatedRoute) { }

    ngOnInit() {
        this.route.parent.paramMap.pipe().subscribe(params => {
            this.tournamentEditionId = +params.get('id');
        });
    }

}

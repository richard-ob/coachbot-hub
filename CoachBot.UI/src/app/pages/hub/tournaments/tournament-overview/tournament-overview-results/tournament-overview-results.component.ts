import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
    selector: 'app-tournament-overview-results',
    templateUrl: './tournament-overview-results.component.html'
})
export class TournamentOverviewResultsComponent implements OnInit {

    tournamentId: number;
    isLoading = true;

    constructor(private route: ActivatedRoute) { }

    ngOnInit() {
        this.route.parent.paramMap.pipe().subscribe(params => {
            this.tournamentId = +params.get('id');
        });
    }

}

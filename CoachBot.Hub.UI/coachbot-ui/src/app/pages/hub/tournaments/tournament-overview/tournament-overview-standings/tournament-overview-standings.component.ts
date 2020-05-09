import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
    selector: 'app-tournament-overview-standings',
    templateUrl: './tournament-overview-standings.component.html'
})
export class TournamentOverviewStandingsComponent implements OnInit {

    tournamenEditionId: number;
    isLoading = true;

    constructor(private route: ActivatedRoute) { }

    ngOnInit() {
        this.route.parent.paramMap.pipe().subscribe(params => {
            this.tournamenEditionId = +params.get('id');
        });
    }

}

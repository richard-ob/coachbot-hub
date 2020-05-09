import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
    selector: 'app-tournament-overview-staff',
    templateUrl: './tournament-overview-staff.component.html'
})
export class TournamentOverviewStaffComponent implements OnInit {

    tournamenEditionId: number;
    isLoading = true;

    constructor(private route: ActivatedRoute) { }

    ngOnInit() {
        this.route.parent.paramMap.pipe().subscribe(params => {
            this.tournamenEditionId = +params.get('id');
        });
    }

}

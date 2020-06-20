import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { FantasyService } from '@pages/hub/shared/services/fantasy.service';

@Component({
    selector: 'app-fantasy-overview',
    templateUrl: './fantasy-overview.component.html'
})
export class FantasyOverviewComponent implements OnInit {

    isCreating = false;
    isLoading = true;

    constructor(private fantasyService: FantasyService, private router: Router) { }

    ngOnInit() {

    }

}

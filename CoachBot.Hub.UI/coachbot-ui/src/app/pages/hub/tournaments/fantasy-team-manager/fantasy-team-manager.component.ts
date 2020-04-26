import { Component } from '@angular/core';

@Component({
    selector: 'app-fantasy-team-manager',
    templateUrl: './fantasy-team-manager.component.html'
})
export class FantasyTeamManagerComponent {

    isCreating = false;
    isLoading = true;

    constructor() { }

}

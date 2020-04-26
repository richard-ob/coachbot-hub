import { Component, OnInit } from '@angular/core';

@Component({
    selector: 'app-current-tournaments',
    templateUrl: './current-tournaments.component.html'
})
export class CurrentTournamentsComponent implements OnInit {

    isLoading = true;

    constructor() { }

    ngOnInit() {
    }
}

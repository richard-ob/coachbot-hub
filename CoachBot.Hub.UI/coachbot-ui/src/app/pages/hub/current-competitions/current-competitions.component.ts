import { Component, OnInit } from '@angular/core';

@Component({
    selector: 'app-current-competitions',
    templateUrl: './current-competitions.component.html'
})
export class CurrentCompetitionsComponent implements OnInit {

    isLoading = true;

    constructor() { }

    ngOnInit() {
    }
}

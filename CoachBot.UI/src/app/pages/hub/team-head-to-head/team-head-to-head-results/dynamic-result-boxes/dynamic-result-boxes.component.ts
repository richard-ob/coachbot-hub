import { Component, OnInit, Input } from '@angular/core';
import { Team } from '@pages/hub/shared/model/team.model';
import ColourUtils from '@shared/utilities/colour-utilities';

@Component({
    selector: 'app-dynamic-result-boxes',
    templateUrl: './dynamic-result-boxes.component.html'
})
export class DynamicResultBoxesComponent implements OnInit {

    @Input() goalsHome: number;
    @Input() goalsAway: number;
    @Input() teamHome: Team;
    @Input() teamAway: Team;
    backgroundColour: string;
    textColour: string;

    constructor() { }

    ngOnInit(): void {
        if (this.goalsHome > this.goalsAway) {
            this.backgroundColour = this.teamHome.color;
        } else if (this.goalsAway > this.goalsHome) {
            this.backgroundColour = this.teamAway.color;
        } else {
            this.backgroundColour = '#e4e4e4';
        }
        this.textColour = this.getTextColour(this.backgroundColour);
    }

    getTextColour(color): string {
        return ColourUtils.isDark(color) ? '#000000' : '#ffffff';
    }

}

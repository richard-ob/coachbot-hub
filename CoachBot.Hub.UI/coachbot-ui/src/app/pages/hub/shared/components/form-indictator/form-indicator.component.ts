import { Component, OnInit, Input } from '@angular/core';
import { MatchOutcomeType } from '../../model/match-outcome-type.enum';

@Component({
    selector: 'app-form-indicator',
    templateUrl: './form-indicator.component.html',
    styleUrls: ['./form-indicator.component.scss']
})
export class FormIndicatorComponent {

    @Input() form: MatchOutcomeType[];
    matchOutcomeType = MatchOutcomeType;

}

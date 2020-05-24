import { Pipe, PipeTransform } from '@angular/core';
import { MatchOutcomeType } from '../model/match-outcome-type.enum';

@Pipe({ name: 'matchOutcome' })
export class MatchOutcomePipe implements PipeTransform {
    transform(matchOutcome: MatchOutcomeType): string {
        switch (matchOutcome) {
            case MatchOutcomeType.Win:
                return 'Win';
            case MatchOutcomeType.Draw:
                return 'Draw';
            case MatchOutcomeType.Loss:
                return 'Loss';
            default:
                return '';
        }
    }
}

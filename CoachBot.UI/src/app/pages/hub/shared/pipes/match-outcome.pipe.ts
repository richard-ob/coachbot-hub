import { Pipe, PipeTransform } from '@angular/core';
import { MatchOutcomeType } from '../model/match-outcome-type.enum';

@Pipe({ name: 'matchOutcome' })
export class MatchOutcomePipe implements PipeTransform {
    transform(matchOutcome: MatchOutcomeType): string {
        switch (matchOutcome) {
            case MatchOutcomeType.Win:
                return $localize`:@@globals.win:Win`;
            case MatchOutcomeType.Draw:
                return $localize`:@@globals.draw:Draw`;
            case MatchOutcomeType.Loss:
                return $localize`:@@globals.loss:Loss`;
            default:
                return '';
        }
    }
}

import { Pipe, PipeTransform } from '@angular/core';
import { TournamentMatchDay } from '../model/tournament-match-day-slot.model';

@Pipe({ name: 'tournamentMatchDay' })
export class TournamentMatchDayPipe implements PipeTransform {
    transform(tournamentMatchDay: TournamentMatchDay): string {
        switch (tournamentMatchDay) {
            case TournamentMatchDay.Monday:
                return $localize`:@@globals.monday:Monday`;
            case TournamentMatchDay.Tuesday:
                return $localize`:@@globals.tuesday:Tuesday`;
            case TournamentMatchDay.Wednesday:
                return $localize`:@@globals.wednesday:Wednesday`;
            case TournamentMatchDay.Thursday:
                return $localize`:@@globals.thursday:Thursday`;
            case TournamentMatchDay.Friday:
                return $localize`:@@globals.friday:Friday`;
            case TournamentMatchDay.Saturday:
                return $localize`:@@globals.saturday:Saturday`;
            case TournamentMatchDay.Sunday:
                return $localize`:@@globals.sunday:Sunday`;
            default:
                return '';
        }
    }
}

import { Pipe, PipeTransform } from '@angular/core';
import { TournamentMatchDay } from '../model/tournament-match-day-slot.model';

@Pipe({ name: 'tournamentMatchDay' })
export class TournamentMatchDayPipe implements PipeTransform {
    transform(tournamentMatchDay: TournamentMatchDay): string {
        switch (tournamentMatchDay) {
            case TournamentMatchDay.Monday:
                return 'Monday';
            case TournamentMatchDay.Tuesday:
                return 'Tuesday';
            case TournamentMatchDay.Wednesday:
                return 'Wednesday';
            case TournamentMatchDay.Thursday:
                return 'Thursday';
            case TournamentMatchDay.Friday:
                return 'Friday';
            case TournamentMatchDay.Saturday:
                return 'Saturday';
            case TournamentMatchDay.Sunday:
                return 'Sunday';
            default:
                return '';
        }
    }
}

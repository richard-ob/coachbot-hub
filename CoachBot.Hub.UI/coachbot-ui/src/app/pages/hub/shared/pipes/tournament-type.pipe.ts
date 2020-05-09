import { Pipe, PipeTransform } from '@angular/core';
import { TournamentType } from '../model/tournament-type.enum';

@Pipe({ name: 'tournamentType' })
export class TournamentTypePipe implements PipeTransform {
    transform(tournamentType: TournamentType): string {
        switch (tournamentType) {
            case TournamentType.RoundRobinLadder:
                return 'Ladder';
            default:
                return '';
        }
    }
}

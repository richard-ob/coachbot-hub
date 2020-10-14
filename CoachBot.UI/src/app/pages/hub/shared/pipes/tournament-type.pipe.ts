import { Pipe, PipeTransform } from '@angular/core';
import { TournamentType } from '../model/tournament-type.enum';

@Pipe({ name: 'tournamentType' })
export class TournamentTypePipe implements PipeTransform {
    transform(tournamentType: TournamentType): string {
        switch (tournamentType) {
            case TournamentType.RoundRobinLadder:
                return 'Ladder';
            case TournamentType.Knockout:
                return 'Knockout';
            case TournamentType.RoundRobinAndKnockout:
                return 'Group Knockout';
            case TournamentType.DoubleRoundRobin:
                return 'League';
            case TournamentType.QuadrupleRoundRobin:
                return 'League';
            default:
                return '';
        }
    }
}

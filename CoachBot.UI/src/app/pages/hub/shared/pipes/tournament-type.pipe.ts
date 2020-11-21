import { Pipe, PipeTransform } from '@angular/core';
import { TournamentType } from '../model/tournament-type.enum';

@Pipe({ name: 'tournamentType' })
export class TournamentTypePipe implements PipeTransform {
    transform(tournamentType: TournamentType): string {
        switch (tournamentType) {
            case TournamentType.RoundRobinLadder:
                return $localize`:@@tournamentTypes.ladder:Ladder`;
            case TournamentType.Knockout:
                return $localize`:@@tournamentTypes.knockout:Knockout`;
            case TournamentType.RoundRobinAndKnockout:
                return $localize`:@@tournamentTypes.groupKnockout:Group Knockout`;
            case TournamentType.DoubleRoundRobin:
                return $localize`:@@tournamentTypes.league:League`;
            case TournamentType.QuadrupleRoundRobin:
                return $localize`:@@tournamentTypes.league:League`;
            default:
                return '';
        }
    }
}

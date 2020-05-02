import { Pipe, PipeTransform } from '@angular/core';
import { TeamRole } from '../model/team-role.enum';

@Pipe({ name: 'teamRole' })
export class TeamRolePipe implements PipeTransform {
    transform(teamRole: TeamRole): string {
        switch (teamRole) {
            case TeamRole.Captain:
                return 'Captain';
            case TeamRole.Loaned:
                return 'Loaned';
            case TeamRole.Loanee:
                return 'Loanee';
            case TeamRole.Player:
                return 'Player';
            case TeamRole.Reserve:
                return 'Reserve';
            case TeamRole.Trialist:
                return 'Trialist';
            case TeamRole.ViceCaptain:
                return 'Vice Captain';
            default:
                return '';
        }
    }
}

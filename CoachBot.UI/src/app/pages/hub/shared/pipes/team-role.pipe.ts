import { Pipe, PipeTransform } from '@angular/core';
import { TeamRole } from '../model/team-role.enum';

@Pipe({ name: 'teamRole' })
export class TeamRolePipe implements PipeTransform {
    transform(teamRole: TeamRole): string {
        switch (teamRole) {
            case TeamRole.Captain:
                return $localize`:@@playerRoles.captain:Captain`;
            case TeamRole.Loaned:
                return $localize`:@@playerRoles.loaned:Loaned`;
            case TeamRole.Loanee:
                return $localize`:@@playerRoles.loanee:Loanee`;
            case TeamRole.Player:
                return $localize`:@@playerRoles.player:Player`;
            case TeamRole.Reserve:
                return $localize`:@@playerRoles.reserve:Reserve`;
            case TeamRole.Trialist:
                return $localize`:@@playerRoles.trialist:Trialist`;
            case TeamRole.ViceCaptain:
                return $localize`:@@playerRoles.viceCaptain:Vice Captain`;
            default:
                return '';
        }
    }
}

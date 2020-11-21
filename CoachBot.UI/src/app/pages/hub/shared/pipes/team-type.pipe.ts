import { Pipe, PipeTransform } from '@angular/core';
import { TeamType } from '../model/team-type.enum';

@Pipe({ name: 'teamType' })
export class TeamTypePipe implements PipeTransform {
    transform(teamType: TeamType): string {
        switch (teamType) {
            case TeamType.Club:
                return $localize`:@@teamTypes.club:Club`;
            case TeamType.Mix:
                return $localize`:@@teamTypes.mx:Mix`;
            case TeamType.National:
                return $localize`:@@teamTypes.national:National`;
            case TeamType.Draft:
                return $localize`:@@teamTypes.draft:League`;
            default:
                return '';
        }
    }
}

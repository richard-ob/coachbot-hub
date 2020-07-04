import { Pipe, PipeTransform } from '@angular/core';
import { TeamType } from '../model/team-type.enum';

@Pipe({ name: 'teamType' })
export class TeamTypePipe implements PipeTransform {
    transform(teamType: TeamType): string {
        switch (teamType) {
            case TeamType.Club:
                return 'Club';
            case TeamType.Mix:
                return 'Mix';
            case TeamType.National:
                return 'National';
            case TeamType.Draft:
                return 'Draft';
            default:
                return '';
        }
    }
}

import { Pipe, PipeTransform } from '@angular/core';
import { FantasyTeamStatus } from '../model/fantasy-team-summary.model';

@Pipe({ name: 'fantasyTeamStatus' })
export class FantasyTeamStatusPipe implements PipeTransform {
    transform(fantasyTeamStatus: FantasyTeamStatus): string {
        switch (fantasyTeamStatus) {
            case FantasyTeamStatus.Open:
                return $localize`:@@globals.open:Open`;
            case FantasyTeamStatus.Pending:
                return $localize`:@@globals.pending:Pending`;
            case FantasyTeamStatus.Settled:
                return $localize`:@@globals.settled:Settled`;
            default:
                return '';
        }
    }
}

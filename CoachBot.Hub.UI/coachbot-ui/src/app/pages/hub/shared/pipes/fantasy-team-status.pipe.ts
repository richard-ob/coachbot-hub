import { Pipe, PipeTransform } from '@angular/core';
import { FantasyTeamStatus } from '../model/fantasy-team-summary.model';

@Pipe({ name: 'fantasyTeamStatus' })
export class FantasyTeamStatusPipe implements PipeTransform {
    transform(fantasyTeamStatus: FantasyTeamStatus): string {
        switch (fantasyTeamStatus) {
            case FantasyTeamStatus.Open:
                return 'Open';
            case FantasyTeamStatus.Pending:
                return 'Pending';
            case FantasyTeamStatus.Settled:
                return 'Settled';
            default:
                return '';
        }
    }
}

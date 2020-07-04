import { Pipe, PipeTransform } from '@angular/core';
import { TournamentStaffRole } from '../model/tournament-staff-role.model';

@Pipe({ name: 'tournamentStaffRole' })
export class TournamentStaffRolePipe implements PipeTransform {
    transform(tournamentStaffRole: TournamentStaffRole): string {
        switch (tournamentStaffRole) {
            case TournamentStaffRole.Organiser:
                return 'Organiser';
            case TournamentStaffRole.MatchAdmin:
                return 'Match Admin';
            case TournamentStaffRole.Streamer:
                return 'Streamer';
            case TournamentStaffRole.Commentator:
                return 'Commentator';
            default:
                return '';
        }
    }
}

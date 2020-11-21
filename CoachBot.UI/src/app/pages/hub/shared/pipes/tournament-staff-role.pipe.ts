import { Pipe, PipeTransform } from '@angular/core';
import { TournamentStaffRole } from '../model/tournament-staff-role.model';

@Pipe({ name: 'tournamentStaffRole' })
export class TournamentStaffRolePipe implements PipeTransform {
    transform(tournamentStaffRole: TournamentStaffRole): string {
        switch (tournamentStaffRole) {
            case TournamentStaffRole.Organiser:
                return $localize`:@@globals.organiser:Organiser`;
            case TournamentStaffRole.MatchAdmin:
                return $localize`:@@globals.matchAdmin:Match Admin`;
            case TournamentStaffRole.Streamer:
                return $localize`:@@globals.streamer:Streamer`;
            case TournamentStaffRole.Commentator:
                return $localize`:@@globals.commentator:Commentator`;
            default:
                return '';
        }
    }
}

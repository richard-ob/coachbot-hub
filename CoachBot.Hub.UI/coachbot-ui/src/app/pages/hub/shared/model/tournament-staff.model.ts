import { Player } from './player.model';
import { TournamentStaffRole } from './tournament-staff-role.model';
import { Tournament } from './tournament.model';

export class TournamentStaff {
    id: number;
    playerId: number;
    player: Player;
    tournamentId: number;
    tournament: Tournament;
    role: TournamentStaffRole;
    createdDate: Date;
}

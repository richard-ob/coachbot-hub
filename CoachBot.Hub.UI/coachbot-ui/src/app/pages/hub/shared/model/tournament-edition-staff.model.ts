import { Player } from './player.model';
import { TournamentEdition } from './tournament-edition.model';
import { TournamentStaffRole } from './tournament-staff-role.model';

export class TournamentEditionStaff {
    id: number;
    playerId: number;
    player: Player;
    tournamentEditionId: number;
    tournamentEdition: TournamentEdition;
    role: TournamentStaffRole;
    createdDate: Date;
}

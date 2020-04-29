import { Player } from './player.model';
import { Team } from './team.model';
import { TeamRole } from './team-role.enum';

export interface PlayerTeam {
    id: number;
    playerId: number;
    player?: Player;
    teamId: number;
    team?: Team;
    teamRole: TeamRole;
    joinDate?: Date;
    leaveDate?: Date;
    isCurrentTeam: boolean;
}

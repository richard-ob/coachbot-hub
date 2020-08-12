import { Country } from './country.model';
import { Team } from './team.model';
import { Position } from './position';
import { TeamRole } from './team-role.enum';

export interface PlayerProfile {
    name: string;
    country: Country;
    clubTeam: Team;
    clubTeamRole: TeamRole;
    nationalTeam: Team;
    position: Position;
}

import { Country } from './country.model';
import { Team } from './team.model';
import { Position } from './position';

export interface PlayerProfile {
    name: string;
    country: Country;
    clubTeam: Team;
    nationalTeam: Team;
    position: Position;
}

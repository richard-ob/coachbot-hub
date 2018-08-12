import { Team } from "./team";
import { Formation } from "./formation";
import { Position } from "./position";

export class Channel {
    id: any;
    idString: string;
    positions: Position[];
    team1: Team;
    team2: Team;
    formation: Formation;
    classicLineup: boolean;
}

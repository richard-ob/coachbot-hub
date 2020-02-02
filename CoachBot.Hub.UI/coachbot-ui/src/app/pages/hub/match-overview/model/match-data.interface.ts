
export interface MatchData {
    statisticTypes: string[];
    matchInfo: MatchInfo;
    teams: TeamElement[];
    players: Player[];
    matchEvents: MatchEvent[];
}

export interface MatchEvent {
    second: number;
    event: EventType;
    period: LastPeriodName;
    team: TeamEnum;
    player1SteamId: string;
    player2SteamId: string;
}

export enum EventType {
    Goal = 'GOAL',
    Miss = 'MISS',
    Save = 'SAVE',
    YellowCard = 'YELLOW CARD',
    RedCard = 'RED CARD',
    SecondYellowCard = 'SECOND YELLOW CARD',
    Penalty = 'PENALTY',
    Foul = 'FOUL',
    OwnGoal = 'OWN GOAL',
    Null = '(null)'
}

export enum LastPeriodName {
    FirstHalf = 'FIRST HALF',
    SecondHalf = 'SECOND HALF',
}

export enum TeamEnum {
    Away = 'away',
    Home = 'home',
}

export interface MatchInfo {
    type: string;
    startTime: number;
    endTime: number;
    periods: number;
    lastPeriodName: LastPeriodName;
}

export interface Player {
    info: PlayerInfo;
    matchPeriodData: MatchPeriodDatum[];
}

export interface PlayerInfo {
    steamId: string;
    name: string;
}

export interface MatchPeriodDatum {
    info: MatchPeriodDatumInfo;
    statistics: number[];
}

export interface MatchPeriodDatumInfo {
    startSecond: number;
    endSecond: number;
    team: TeamEnum;
    position: string;
}

export interface TeamElement {
    matchTotal: MatchTotal;
    matchPeriods: MatchPeriod[];
}

export interface MatchPeriod {
    period: number;
    periodName: LastPeriodName;
    announcedInjuryTimeSeconds: number;
    actualInjuryTimeSeconds: number;
    statistics: number[];
}

export interface MatchTotal {
    name: string;
    side: TeamEnum;
    isMix: boolean;
    statistics: number[];
}

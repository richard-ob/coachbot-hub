import { MatchFormat } from './match-format.enum';

export class Region {
    regionId: number;
    regionName: string;
    regionCode: string;
    matchFormat: MatchFormat;
    createdDate: Date;
}

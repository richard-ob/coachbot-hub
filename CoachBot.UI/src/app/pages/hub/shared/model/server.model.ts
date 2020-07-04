import { Region } from './region.model';
import { Country } from './country.model';

export class Server {
    id: number;
    name: string;
    address: string;
    rconPassword: string;
    regionId: number;
    region: Region;
    countryId: number;
    country: Country;
    createdDate: Date;
}

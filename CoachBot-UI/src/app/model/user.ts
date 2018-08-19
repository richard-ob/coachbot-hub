import { Channel } from './channel';

export class User {
    public name: string;
    public discordUserIdString: string;
    public id: number;
    public IsAdministrator: boolean;
    public channels: Channel[];
}

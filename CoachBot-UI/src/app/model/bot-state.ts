import { Guild } from "./guild";

export class BotState {
    connectionStatus: string;
    loginStatus: string;
    guilds: Guild[];
}


import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { Injectable } from '@angular/core';
import { Guild } from '../model/guild';
import { DiscordGuild } from '../model/discord-guild.model';
import { DiscordChannel } from '../model/discord-channel.model';

@Injectable({
    providedIn: 'root'
})
export class DiscordService {

    constructor(private http: HttpClient) { }

    getGuildsForUser(): Observable<DiscordGuild[]> {
        return this.http.get<DiscordGuild[]>(`${environment.apiUrl}/api/discordguild`);
    }

    getChannelsForGuild(discordGuildId: string): Observable<DiscordChannel[]> {
        return this.http.get<DiscordChannel[]>(`${environment.apiUrl}/api/discordguild/${discordGuildId}/channels`);
    }

}

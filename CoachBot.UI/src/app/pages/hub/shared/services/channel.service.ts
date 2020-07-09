
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { Injectable } from '@angular/core';
import { Channel } from '../model/channel.model';

@Injectable({
    providedIn: 'root'
})
export class ChannelService {

    constructor(private http: HttpClient) { }

    getChannel(channelId: number): Observable<Channel> {
        return this.http.get<Channel>(`${environment.apiUrl}/api/channel/${channelId}`);
    }

    getChannelsForGuild(discordGuildId: string): Observable<Channel[]> {
        return this.http.get<Channel[]>(`${environment.apiUrl}/api/guild/${discordGuildId}/channels`);
    }

    getChannelsForTeamGuild(discordGuildId: string, teamId: number): Observable<Channel[]> {
        return this.http.get<Channel[]>(`${environment.apiUrl}/api/guild/${discordGuildId}/${teamId}/channels`);
    }

    createChannel(channel: Channel): Observable<void> {
        return this.http.post<void>(`${environment.apiUrl}/api/channel`, channel);
    }

    updateChannel(channel: Channel): Observable<void> {
        return this.http.put<void>(`${environment.apiUrl}/api/channel`, channel);
    }

}

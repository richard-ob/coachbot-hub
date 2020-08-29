import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { BotState } from '../model/bot-state.model';
import { Announcement } from '../model/announcement.model';

@Injectable({
    providedIn: 'root'
})
export class BotService {

    constructor(private http: HttpClient) { }

    getBotState(): Observable<BotState> {
        return this.http.get<BotState>(`${environment.apiUrl}/api/bot/state`).pipe();
    }

    getBotLogs(): Observable<string> {
        return this.http.get<string>(`${environment.apiUrl}/api/bot/logs`).pipe();
    }

    reconnectBot() {
        return this.http.post(`${environment.apiUrl}/api/bot/reconnect`, null).pipe();
    }

    connectBot() {
        return this.http.post(`${environment.apiUrl}/api/bot/connect`, null).pipe();
    }

    disconnectBot() {
        return this.http.post(`${environment.apiUrl}/api/bot/disconnect`, null).pipe();
    }

    sendAnnouncement(announcement: Announcement) {
        return this.http.post(`${environment.apiUrl}/api/announcement`, announcement).pipe();
    }
}

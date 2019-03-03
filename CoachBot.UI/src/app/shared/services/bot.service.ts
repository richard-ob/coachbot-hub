import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { BotState } from '../../model/bot-state';
import { environment } from '../../../environments/environment';

@Injectable({
    providedIn: 'root'
})
export class BotService {

    constructor(private http: HttpClient) { }

    getBotState(): Observable<BotState> {
        return this.http.get<BotState>(`${environment.apiUrl}/api/botstate`).pipe();
    }

    reconnectBot() {
        return this.http.post(`${environment.apiUrl}/api/botstate/reconnect`, null).pipe();
    }

    leaveGuild(id: number) {
        return this.http.delete(`${environment.apiUrl})/api/guild/${id}`).pipe();
    }
}

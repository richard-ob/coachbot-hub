import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { BotState } from '../../model/bot-state';

@Injectable({
    providedIn: 'root'
})
export class BotService {

    constructor(private http: HttpClient) { }

    getBotState(): Observable<BotState> {
        return this.http.get<BotState>('http://localhost:5006/api/botstate').pipe();
    }

    reconnectBot() {
        return this.http.post('http://localhost:5006/api/botstate/reconnect', null).pipe();
    }

    leaveGuild(id: number) {
        return this.http.delete(`http://localhost:5006/api/guild/${id}`).pipe();
    }
}

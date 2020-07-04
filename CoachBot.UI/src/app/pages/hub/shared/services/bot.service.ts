import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { BotState } from '../model/bot-state.model';

@Injectable({
    providedIn: 'root'
})
export class BotService {

    constructor(private http: HttpClient) { }

    getBotState(): Observable<BotState> {
        return this.http.get<BotState>(`${environment.apiUrl}/api/bot/state`).pipe();
    }

    getBotLogs(): Observable<string> {
        return this.http.get<string>(`${environment.apiUrl}/api/log/`).pipe();
    }

    reconnectBot() {
        return this.http.post(`${environment.apiUrl}/api/bot/reconnect`, null).pipe();
    }
}

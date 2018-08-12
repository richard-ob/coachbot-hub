import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { Match } from '../../model/match';

@Injectable({
    providedIn: 'root'
})
export class MatchService {

    constructor(private http: HttpClient) { }

    getMatchHistory(channelId: string): Observable<Match[]> {
        return this.http.get<Match[]>(`http://localhost:5006/api/match/channel/${channelId}`).pipe();
    }
}

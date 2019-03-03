import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { Match } from '../../model/match';
import { environment } from '../../../environments/environment';

@Injectable({
    providedIn: 'root'
})
export class MatchService {

    constructor(private http: HttpClient) { }

    getMatchHistory(channelId: string): Observable<Match[]> {
        return this.http.get<Match[]>(`${environment.apiUrl}/api/match/channel/${channelId}`).pipe();
    }
}

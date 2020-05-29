
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { Injectable } from '@angular/core';
import { ScorePrediction } from '../model/score-prediction.model';
import { ScorePredictionLeaderboardPlayer } from '../model/score-prediction-leaderboard-player.model';
@Injectable({
    providedIn: 'root'
})
export class ScorePredictorService {

    constructor(private http: HttpClient) { }

    getScorePredictions(tournamentEditionId: number): Observable<ScorePrediction[]> {
        return this.http.get<ScorePrediction[]>(`${environment.apiUrl}/api/score-predictions/tournament/${tournamentEditionId}`);
    }

    getScorePredictionsForPlayer(tournamentEditionId: number, playerId: number): Observable<ScorePrediction[]> {
        return this.http.get<ScorePrediction[]>(
            `${environment.apiUrl}/api/score-predictions/tournament/${tournamentEditionId}/player/${playerId}`
        );
    }

    getScorePredictionLeaderboard(tournamentEditionId: number): Observable<ScorePredictionLeaderboardPlayer[]> {
        return this.http.get<ScorePredictionLeaderboardPlayer[]>(
            `${environment.apiUrl}/api/score-predictions/tournament/${tournamentEditionId}/leaderboard`
        );
    }

    createScorePrediction(scorePrediction: ScorePrediction): Observable<void> {
        return this.http.post<void>(`${environment.apiUrl}/api/score-predictions`, scorePrediction);
    }

    updateFantasyTeam(scorePrediction: ScorePrediction): Observable<void> {
        return this.http.put<void>(`${environment.apiUrl}/api/score-predictions`, scorePrediction);
    }

}

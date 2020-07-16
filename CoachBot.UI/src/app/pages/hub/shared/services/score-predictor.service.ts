
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

    getScorePredictions(tournamentId: number): Observable<ScorePrediction[]> {
        return this.http.get<ScorePrediction[]>(`${environment.apiUrl}/api/score-predictions/tournament/${tournamentId}`);
    }

    getScorePredictionsForPlayer(tournamentId: number, playerId: number): Observable<ScorePrediction[]> {
        return this.http.get<ScorePrediction[]>(
            `${environment.apiUrl}/api/score-predictions/tournament/${tournamentId}/player/${playerId}`
        );
    }

    getHistoricScorePredictionsForPlayer(playerId: number): Observable<ScorePrediction[]> {
        return this.http.get<ScorePrediction[]>(
            `${environment.apiUrl}/api/score-predictions/player/${playerId}`
        );
    }

    getScorePredictionMonthLeader(): Observable<ScorePredictionLeaderboardPlayer> {
        return this.http.get<ScorePredictionLeaderboardPlayer>(
            `${environment.apiUrl}/api/score-predictions/leaderboard/month-leader`
        );
    }

    getScorePredictionLeaderboard(tournamentId: number): Observable<ScorePredictionLeaderboardPlayer[]> {
        return this.http.get<ScorePredictionLeaderboardPlayer[]>(
            `${environment.apiUrl}/api/score-predictions/tournament/${tournamentId}/leaderboard`
        );
    }

    getScorePredictionsGlobalLeaderboard(): Observable<ScorePredictionLeaderboardPlayer[]> {
        return this.http.get<ScorePredictionLeaderboardPlayer[]>(
            `${environment.apiUrl}/api/score-predictions/leaderboard`
        );
    }

    createScorePrediction(scorePrediction: ScorePrediction): Observable<void> {
        return this.http.post<void>(`${environment.apiUrl}/api/score-predictions`, scorePrediction);
    }

    updateFantasyTeam(scorePrediction: ScorePrediction): Observable<void> {
        return this.http.put<void>(`${environment.apiUrl}/api/score-predictions`, scorePrediction);
    }

}

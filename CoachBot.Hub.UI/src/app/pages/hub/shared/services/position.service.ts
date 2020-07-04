
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { Injectable } from '@angular/core';
import { Position } from '../model/position';

@Injectable({
    providedIn: 'root'
})
export class PositionService {

    constructor(private http: HttpClient) { }

    getPositions(): Observable<Position[]> {
        return this.http.get<Position[]>(`${environment.apiUrl}/api/position`);
    }
}

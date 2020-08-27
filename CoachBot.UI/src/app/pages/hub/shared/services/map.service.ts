
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { Injectable } from '@angular/core';
import { Map } from '../model/map.model';

@Injectable({
    providedIn: 'root'
})
export class MapService {

    constructor(private http: HttpClient) { }

    getMaps(): Observable<Map[]> {
        return this.http.get<Map[]>(`${environment.apiUrl}/api/map`);
    }
}

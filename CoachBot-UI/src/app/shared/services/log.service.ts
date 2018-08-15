import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';

@Injectable({
    providedIn: 'root'
})
export class LogService {

    constructor(private http: HttpClient) { }

    getLog(): Observable<string> {
        return this.http.get<string>(`http://localhost:5006/api/log/`).pipe();
    }
}

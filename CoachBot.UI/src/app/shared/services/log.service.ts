import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';

@Injectable({
    providedIn: 'root'
})
export class LogService {

    constructor(private http: HttpClient) { }

    getLog(): Observable<string> {
        return this.http.get<string>(`${environment.apiUrl}/api/log/`).pipe();
    }
}

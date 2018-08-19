import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';

@Injectable({
    providedIn: 'root'
})
export class UserService {

    constructor(private http: HttpClient) { }

    getUser(): Observable<any> {
        return this.http.get(`${environment.apiUrl}/api/user`).pipe();
    }

    getUserStatistics(): Observable<any> {
        return this.http.get(`${environment.apiUrl}/api/user/statistics`).pipe();
    }
}

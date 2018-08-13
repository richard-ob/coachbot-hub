import { Injectable } from '@angular/core';
import { Configuration } from '../../model/configuration';
import { Observable, of, Observer } from 'rxjs';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { ChatMessage } from '../../model/chat-message';

@Injectable({
    providedIn: 'root'
})
export class UserService {

    constructor(private http: HttpClient) { }

    getUser(): Observable<any> {
        return this.http.get('http://localhost:5006/api/user').pipe();
    }

    getUserStatistics(): Observable<any> {
        return this.http.get('http://localhost:5006/api/user/statistics').pipe();
    }
}

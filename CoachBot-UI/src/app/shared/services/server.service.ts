import { Injectable } from '@angular/core';
import { Server } from '../../model/server';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';

@Injectable({
    providedIn: 'root'
})
export class ServerService {

    constructor(private http: HttpClient) { }

    getServers(): Observable<Server[]> {
        return this.http.get<Server[]>('http://localhost:5006/api/server').pipe();
    }

    getServer(id: number): Observable<Server> {
        return this.http.get<Server>(`http://localhost:5006/api/server/${id}`).pipe();
    }

    addServer(server: Server) {
        return this.http.post('http://localhost:5006/api/server', server).pipe();
    }

    removeServer(id: number) {
        return this.http.delete(`http://localhost:5006/api/server/${id}`).pipe();
    }
}

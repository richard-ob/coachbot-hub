import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Server } from '../model/server.model';
import { environment } from 'src/environments/environment';

@Injectable({
    providedIn: 'root'
})
export class ServerService {

    constructor(private http: HttpClient) { }

    getServers(): Observable<Server[]> {
        return this.http.get<Server[]>(`${environment.apiUrl}/api/server`).pipe();
    }

    getServer(id: number): Observable<Server> {
        return this.http.get<Server>(`${environment.apiUrl}/api/server/${id}`).pipe();
    }

    addServer(server: Server) {
        return this.http.post(`${environment.apiUrl}/api/server`, server).pipe();
    }

    updateServer(server: Server) {
        return this.http.put(`${environment.apiUrl}/api/server`, server).pipe();
    }

    updateServerRconPassword(serverId: number, rconPassword: string) {
        const formData = new FormData();
        if (rconPassword && rconPassword.length) {
            formData.append('rconPassword', rconPassword);
        }
        return this.http.patch(`${environment.apiUrl}/api/server/${serverId}`, formData).pipe();
    }

    removeServer(id: number) {
        return this.http.delete(`${environment.apiUrl}/api/server/${id}`).pipe();
    }
}

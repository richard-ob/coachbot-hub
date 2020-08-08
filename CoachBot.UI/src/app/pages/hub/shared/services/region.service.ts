
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { Region } from '../model/region.model';
import { Injectable } from '@angular/core';
import { shareReplay } from 'rxjs/operators';

@Injectable({
    providedIn: 'root'
})
export class RegionService {

    constructor(private http: HttpClient) { }

    getRegions(): Observable<Region[]> {
        return this.http.get<Region[]>(`${environment.apiUrl}/api/region`).pipe(shareReplay({ bufferSize: 1, refCount: true }));
    }

    addRegion(region: Region) {
        return this.http.post(`${environment.apiUrl}/api/region`, region).pipe();
    }

    removeRegion(regionId: number) {
        return this.http.delete(`${environment.apiUrl}/api/region/${regionId}`).pipe();
    }
}

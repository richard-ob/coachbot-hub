
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { Injectable } from '@angular/core';
import { Country } from '../model/country.model';

@Injectable({
    providedIn: 'root'
})
export class CountryService {

    constructor(private http: HttpClient) { }

    getCountries(): Observable<Country[]> {
        return this.http.get<Country[]>(`${environment.apiUrl}/api/country`);
    }
}

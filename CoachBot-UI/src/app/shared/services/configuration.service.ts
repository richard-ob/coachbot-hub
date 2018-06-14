import { Injectable } from '@angular/core';
import { Configuration } from '../../model/configuration';
import { Observable, of } from 'rxjs';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class ConfigurationService {

  constructor(private http: HttpClient) { }

  getConfiguration(): Observable<Configuration> {
    return this.http.get<Configuration>("http://localhost:5006/api/config")
    .pipe();
  }
}

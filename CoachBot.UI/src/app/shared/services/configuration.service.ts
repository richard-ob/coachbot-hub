import { Injectable } from '@angular/core';
import { Configuration } from '../../model/configuration';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ConfigurationService {

  constructor(private http: HttpClient) { }

  getConfiguration(): Observable<Configuration> {
    return this.http.get<Configuration>(`${environment.apiUrl}/api/config`).pipe();
  }

  updateConfiguration(config: Configuration) {
    return this.http.post(`${environment.apiUrl}/api/config`, config).pipe();
  }
}

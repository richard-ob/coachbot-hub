import { Injectable } from '@angular/core';
import { Channel } from '../../model/channel';
import { Observable, of } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ChannelService {

  constructor(private http: HttpClient) { }

  getChannels(): Observable<Channel[]> {
    return this.http.get<Channel[]>(`${environment.apiUrl}/api/channel/`)
      .pipe();
  }

  getChannel(id: string): Observable<Channel> {
    return this.http.get<Channel>(`${environment.apiUrl}/api/channel/${id}`)
      .pipe();
  }

  getUnconfiguredChannels(): Observable<Channel[]> {
    return this.http.get<Channel[]>(`${environment.apiUrl}/api/channel/unconfigured`)
      .pipe();
  }

  updateChannel(channel: Channel) {
    return this.http.post(`${environment.apiUrl}/api/channel`, channel).pipe();
  }
}

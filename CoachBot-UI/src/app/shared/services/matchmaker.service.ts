import { Injectable } from '@angular/core';
import { Channel } from '../../model/channel';
import { Observable, of } from 'rxjs';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class MatchmakerService {

  constructor(private http: HttpClient) { }

  getChannels(): Observable<Channel[]> {
    return this.http.get<Channel[]>('http://localhost:5006/api/channel/')
      .pipe();
  }

  getUnconfiguredChannels(): Observable<Channel[]> {
    return this.http.get<Channel[]>('http://localhost:5006/api/channel/unconfigured')
      .pipe();
  }

  updateChannel(channel: Channel) {
    return this.http.post('http://localhost:5006/api/channel', channel).subscribe();
  }
}

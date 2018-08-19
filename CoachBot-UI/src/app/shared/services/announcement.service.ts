import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { ChatMessage } from '../../model/chat-message';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AnnouncementService {

  httpOptions = {
    headers: new HttpHeaders({
      'Content-Type': 'application/json'
    })
  };

  constructor(private http: HttpClient) {
  }

  sendAnnouncement(message: ChatMessage) {
    return this.http.post(`${environment.apiUrl}/api/announcement/`, message).pipe();
  }
}

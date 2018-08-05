import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { ChatMessage } from '../../model/chat-message';

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
    return this.http.post('http://localhost:5006/api/announcement/', message).pipe();
  }
}

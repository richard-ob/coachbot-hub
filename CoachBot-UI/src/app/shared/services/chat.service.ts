import { Injectable } from '@angular/core';
import { Configuration } from '../../model/configuration';
import { Observable, of, Observer } from 'rxjs';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { ChatMessage } from '../../model/chat-message';

@Injectable({
  providedIn: 'root'
})
export class ChatService {
  
  httpOptions = {
    headers: new HttpHeaders({
      'Content-Type':  'application/json'
    })
  };

  constructor(private http: HttpClient) { 
  }

  sendMessage(message: ChatMessage) {
    return this.http.post("http://localhost:5006/api/Chat/", message).subscribe();
  }
}

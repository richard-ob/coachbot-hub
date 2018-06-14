import { Component } from '@angular/core';
import { ConfigurationService } from './shared/services/configuration.service';
import { Configuration } from './model/configuration';
import { ChatService } from './shared/services/chat.service';
import { ChatMessage } from './model/chat-message';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {

  title = 'app';
  configuration: Configuration;
  message: ChatMessage = new ChatMessage();

  constructor(private configurationService: ConfigurationService, private chatService: ChatService) {
    this.configurationService.getConfiguration().subscribe(configuration => this.configuration = configuration);
  }

  public sendMessage(){
    this.message.sender = "Richard";
    this.chatService.sendMessage(this.message);
  }

}

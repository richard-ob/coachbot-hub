import { Component } from '@angular/core';
import { ConfigurationService } from './shared/services/configuration.service';
import { Configuration } from './model/configuration';
import { ChatService } from './shared/services/chat.service';
import { ChatMessage } from './model/chat-message';
import { MatchmakerService } from './shared/services/matchmaker.service';
import { Channel } from './model/channel';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {

  title = 'app';
  configuration: Configuration;
  message: ChatMessage = new ChatMessage();
  channels: Channel[];

  constructor(private configurationService: ConfigurationService, private chatService: ChatService, private matchmakerService: MatchmakerService) {
    this.configurationService.getConfiguration().subscribe(configuration => this.configuration = configuration);
    this.matchmakerService.getChannels().subscribe(channels => this.channels = channels);
  }

  public sendMessage(){
    this.message.sender = "Richard";
    this.chatService.sendMessage(this.message);
  }

}

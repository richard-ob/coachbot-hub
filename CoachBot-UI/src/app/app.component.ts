import { Component } from '@angular/core';
import { ConfigurationService } from './shared/services/configuration.service';
import { Configuration } from './model/configuration';
import { ChatMessage } from './model/chat-message';
import { MatchmakerService } from './shared/services/matchmaker.service';
import { Channel } from './model/channel';
import { UserService } from './shared/services/user.service';
import { ServerService } from './shared/services/server.service';
import { AnnouncementService } from './shared/services/announcement.service';

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
  user: any;

  constructor(private configurationService: ConfigurationService,
    private announcementService: AnnouncementService,
    private matchmakerService: MatchmakerService,
    private userService: UserService,
    private serverService: ServerService) {
    this.configurationService.getConfiguration().subscribe(configuration => this.configuration = configuration);
    this.matchmakerService.getChannels().subscribe(channels => this.channels = channels);
    this.userService.getUser().subscribe(user => this.user = user);
  }
}

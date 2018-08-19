import { Component } from '@angular/core';
import { ConfigurationService } from './shared/services/configuration.service';
import { Configuration } from './model/configuration';
import { ChatMessage } from './model/chat-message';
import { MatchmakerService } from './shared/services/matchmaker.service';
import { Channel } from './model/channel';
import { UserService } from './shared/services/user.service';
import { environment } from '../environments/environment';

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
  apiUrl = environment.apiUrl;
  onFeet = false;

  constructor(private configurationService: ConfigurationService,
    private matchmakerService: MatchmakerService,
    private userService: UserService) {
    this.configurationService.getConfiguration().subscribe(configuration => this.configuration = configuration);
    this.matchmakerService.getChannels().subscribe(channels => this.channels = channels);
    this.userService.getUser().subscribe(user => this.user = user);
  }

  // Super important method for making Coach's head fly around the screen
  stayOnYourFeet() {
    this.onFeet = !this.onFeet;
    console.log('Stay on your feet!');
  }
}

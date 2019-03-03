import { Component } from '@angular/core';
import { ChannelService } from './shared/services/channel.service';
import { Channel } from './model/channel';
import { UserService } from './shared/services/user.service';
import { environment } from '../environments/environment';
import { User } from './model/user';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {

  channels: Channel[];
  user: User;
  apiUrl = environment.apiUrl;
  onFeet = false;

  constructor(private channelService: ChannelService,
    private userService: UserService) {
    this.channelService.getChannels().subscribe(channels => this.channels = channels);
    this.userService.getUser().subscribe(user => this.user = user);
  }

  // Super important method for making Coach's head fly around the screen
  stayOnYourFeet() {
    this.onFeet = !this.onFeet;
    console.log('Stay on your feet!');
  }
}

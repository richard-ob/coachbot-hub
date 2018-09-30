import { Component } from '@angular/core';
import { MatchmakerService } from './shared/services/matchmaker.service';
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

  constructor(private matchmakerService: MatchmakerService,
    private userService: UserService) {
    this.matchmakerService.getChannels().subscribe(channels => this.channels = channels);
    this.userService.getUser().subscribe(user => this.user = user);
  }

  // Super important method for making Coach's head fly around the screen
  stayOnYourFeet() {
    this.onFeet = !this.onFeet;
    console.log('Stay on your feet!');
  }
}

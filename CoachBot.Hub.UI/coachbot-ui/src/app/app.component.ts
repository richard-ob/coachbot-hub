import { Component } from '@angular/core';
import { UserService } from './core/services/user.service';
import { User } from './core/models/user.model';
import { environment } from 'src/environments/environment';
import { Player } from './pages/hub/shared/model/player.model';
import { PlayerService } from './pages/hub/shared/services/player.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {

  player: Player;
  apiUrl = environment.apiUrl;

  constructor(private playerService: PlayerService) {
    this.playerService.getCurrentPlayer().subscribe(player => {
      this.player = player;
    });
  }

}

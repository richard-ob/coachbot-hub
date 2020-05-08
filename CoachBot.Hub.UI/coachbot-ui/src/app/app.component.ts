import { Component, ElementRef, OnInit } from '@angular/core';
import { UserService } from './core/services/user.service';
import { User } from './core/models/user.model';
import { environment } from 'src/environments/environment';
import { Player } from './pages/hub/shared/model/player.model';
import { PlayerService } from './pages/hub/shared/services/player.service';
import { BreakpointObserver, BreakpointState, Breakpoints } from '@angular/cdk/layout';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {

  player: Player;
  apiUrl = environment.apiUrl;

  constructor(private playerService: PlayerService) { }

  ngOnInit() {
    this.playerService.getCurrentPlayer().subscribe(player => {
      this.player = player;
    });
  }


  toggleSectionOpen(section: Element, toggle: Element) {
    section.classList.toggle('main-nav__section--opened');
    toggle.classList.toggle('main-nav__toggle--rotate');
  }

  toggleSidebar() {
    const menu = document.querySelector('.main-nav');
    const backdrop = document.querySelector('.main-nav--backdrop');
    if (menu.classList.contains('main-nav--opened')) {
      menu.classList.remove('main-nav--opened');
      backdrop.classList.add('d-none');
    } else {
      menu.classList.add('main-nav--opened');
      backdrop.classList.remove('d-none');
    }
  }
}

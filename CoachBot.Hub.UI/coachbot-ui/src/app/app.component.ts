import { Component, ElementRef, OnInit } from '@angular/core';
import { User } from './core/models/user.model';
import { environment } from 'src/environments/environment';
import { Player } from './pages/hub/shared/model/player.model';
import { PlayerService } from './pages/hub/shared/services/player.service';
import { BreakpointObserver, BreakpointState, Breakpoints } from '@angular/cdk/layout';
import { UserPreferenceService, UserPreferenceType } from '@shared/services/user-preferences.service';
import { Region } from '@pages/hub/shared/model/region.model';
import { RegionService } from '@pages/hub/shared/services/region.service';
import { Router, NavigationEnd } from '@angular/router';
import { filter } from 'rxjs/operators';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {

  selectedRegion: Region;
  regions: Region[];
  player: Player;
  apiUrl = environment.apiUrl;

  constructor(
    private playerService: PlayerService,
    private userPreferenceService: UserPreferenceService,
    private regionService: RegionService,
    private router: Router
  ) { }

  ngOnInit() {
    this.regionService.getRegions().subscribe(regions => {
      this.regions = regions;
      this.selectedRegion = regions.find(r => r.regionId === this.userPreferenceService.getUserPreference(UserPreferenceType.Region));
      this.playerService.getCurrentPlayer().subscribe(player => {
        this.player = player;
      });
    });
    this.router.events.pipe(filter((e): e is NavigationEnd => e instanceof NavigationEnd)).subscribe(() => {
      console.log('closing');
      this.closeSidebar();
    });
  }

  setRegion(regionId: number) {
    this.userPreferenceService.setUserPreference(UserPreferenceType.Region, regionId);
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

  closeSidebar() {
    const menu = document.querySelector('.main-nav');
    const backdrop = document.querySelector('.main-nav--backdrop');
    if (menu.classList.contains('main-nav--opened')) {
      menu.classList.remove('main-nav--opened');
      backdrop.classList.add('d-none');
    }
  }
}

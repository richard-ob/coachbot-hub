import { Component, OnInit, Input } from '@angular/core';
import { Router } from '@angular/router';
import { Player } from '@pages/hub/shared/model/player.model';

@Component({
    selector: 'app-player-of-the-match',
    templateUrl: './player-of-the-match.component.html'
})
export class PlayerOfTheMatchComponent implements OnInit {

    @Input() playerOfTheMatch: Player;
    isLoading = false;

    constructor(private router: Router) { }

    ngOnInit() { }

    navigateToPlayer() {
        this.router.navigate(['/player-profile/', this.playerOfTheMatch.id]);
    }

}

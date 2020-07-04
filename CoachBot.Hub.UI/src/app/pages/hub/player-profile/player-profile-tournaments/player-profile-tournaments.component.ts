import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { PlayerService } from '../../shared/services/player.service';

@Component({
    selector: 'app-player-profile-tournaments',
    templateUrl: './player-profile-tournaments.component.html'
})
export class PlayerProfileTournamentsComponent implements OnInit {

    isLoading = true;

    constructor(private route: ActivatedRoute, private playerService: PlayerService) { }

    ngOnInit() {
        this.route.parent.paramMap.pipe().subscribe(params => {
            const playerId = +params.get('id');
        });
    }
}

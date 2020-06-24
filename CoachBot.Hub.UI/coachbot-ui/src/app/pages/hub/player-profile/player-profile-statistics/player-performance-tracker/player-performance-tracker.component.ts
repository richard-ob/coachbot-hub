import { Component, OnInit, Input } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { PlayerService } from '../../../shared/services/player.service';

@Component({
    selector: 'app-player-performance-tracker',
    templateUrl: './player-performance-tracker.component.html'
})
export class PlayerPerformanceTrackerComponent implements OnInit {

    @Input() playerId: number;
    isLoading = true;

    constructor(private route: ActivatedRoute, private playerService: PlayerService) { }

    ngOnInit() {

    }
}

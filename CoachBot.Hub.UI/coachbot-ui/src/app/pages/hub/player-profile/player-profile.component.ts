import { Component, OnInit } from '@angular/core';
import { Player } from '../shared/model/player.model';
import { PlayerService } from '../shared/services/player.service';
import { ActivatedRoute } from '@angular/router';
import { SteamService } from '../shared/services/steam.service.';
import { SteamUserProfile } from '../shared/model/steam-user-profile.model';
import * as humanizeDuration from 'humanize-duration';
import { Match } from '../shared/model/match.model';
import { MatchService } from '../shared/services/match.service';

@Component({
    selector: 'app-player-profile',
    templateUrl: './player-profile.component.html',
    styleUrls: ['./player-profile.component.scss']
})
export class PlayerProfileComponent implements OnInit {

    player: Player;
    steamUserProfile: SteamUserProfile;
    playingTime: string;
    isLoading = true;
    matches: Match[];
    currentPage = 1;
    totalPages: number;
    totalItems: number;

    constructor(
        private playerService: PlayerService,
        private steamService: SteamService,
        private matchService: MatchService,
        private route: ActivatedRoute) { }

    ngOnInit() {
        this.route.paramMap.pipe().subscribe(params => {
            this.playerService.getPlayer(+params.get('id')).subscribe(response => {
                this.player = response;
                this.steamService.getUserProfiles([this.player.steamID]).subscribe(steamUserProfile => {
                    this.steamUserProfile = steamUserProfile.response.players[0];
                    this.steamService.getPlayingTime(this.player.steamID).subscribe(steamPlayingTime => {
                        if (steamPlayingTime && steamPlayingTime.response && steamPlayingTime.response.games) {
                            const iosoccerStats = steamPlayingTime.response.games.find(g => g.appid === 673560);
                            if (iosoccerStats && iosoccerStats.playtime_forever) {
                                this.playingTime = humanizeDuration(iosoccerStats.playtime_forever * 60 * 1000);
                            }
                        }
                        this.isLoading = false;
                    });
                });
            });
        });
    }
}

import { Component, OnInit } from '@angular/core';
import { Player } from '../shared/model/player.model';
import { PlayerService } from '../shared/services/player.service';
import { ActivatedRoute } from '@angular/router';
import { SteamService } from '../shared/services/steam.service.';
import { SteamUserProfile } from '../shared/model/steam-user-profile.model';
import * as humanizeDuration from 'humanize-duration';
import { PlayerProfile } from '../shared/model/player-profile.model';

/*
    IDEAS:
    - Use a better background for the player profile screen. Map doesn't really cut the mustard.
    Copy https://www.premierleague.com/players/4852/Adri%C3%A1n/stats
*/
@Component({
    selector: 'app-player-profile',
    templateUrl: './player-profile.component.html',
    styleUrls: ['./player-profile.component.scss']
})
export class PlayerProfileComponent implements OnInit {

    player: Player;
    playerProfile: PlayerProfile;
    steamUserProfile: SteamUserProfile;
    playingTime: string;
    isLoading = true;

    constructor(
        private playerService: PlayerService,
        private steamService: SteamService,
        private route: ActivatedRoute
    ) { }

    ngOnInit() {
        this.route.paramMap.pipe().subscribe(params => {
            this.playerService.getPlayer(+params.get('id')).subscribe(player => {
                this.player = player;
                this.playerService.getPlayerProfile(player.id).subscribe(playerProfile => {
                    this.playerProfile = playerProfile;
                    if (this.player.steamID) {
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
                    } else {
                        this.steamUserProfile = null;
                        this.isLoading = false;
                    }
                });
            });
        });
    }
}

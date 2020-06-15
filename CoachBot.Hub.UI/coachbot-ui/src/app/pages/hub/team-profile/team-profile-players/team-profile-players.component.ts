import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { TeamService } from '../../shared/services/team.service';
import { PlayerTeamStatisticsTotals } from '../../shared/model/player-team-statistics-totals.model';
import { SteamService } from '@shared/services/steam.service.';

@Component({
    selector: 'app-team-profile-players',
    templateUrl: './team-profile-players.component.html'
})
export class TeamProfilePlayersComponent implements OnInit {

    playerTeamStatisticsTotals: PlayerTeamStatisticsTotals[];
    isLoading = true;

    constructor(
        private route: ActivatedRoute,
        private teamService: TeamService,
        private router: Router,
        private steamService: SteamService
    ) { }

    ngOnInit() {
        this.route.parent.paramMap.pipe().subscribe(params => {
            const teamId = +params.get('id');
            this.teamService.getTeamSquad(teamId).subscribe((playerTeamStatisticsTotals) => {
                this.playerTeamStatisticsTotals = playerTeamStatisticsTotals;
                this.getSteamUserProfiles(this.playerTeamStatisticsTotals);
            });
        });
    }

    navigateToPlayerProfile(playerId: number) {
        this.router.navigate(['/player-profile/', playerId]);
    }


    getSteamUserProfiles(playerTeamStatisticsTotals: PlayerTeamStatisticsTotals[]) {
        const steamIds = [];
        for (const player of playerTeamStatisticsTotals) {
            if (player.playerTeam && player.playerTeam.player.steamID) {
                steamIds.push(player.playerTeam.player.steamID);
            }
        }
        this.steamService.getUserProfiles(steamIds).subscribe(response => {
            for (const player of playerTeamStatisticsTotals) {
                if (player.playerTeam.player.steamID && player.playerTeam.player.steamID.length > 5) {
                    const steamUserProfile = response.response.players.find(u => u.steamid === player.playerTeam.player.steamID);
                    if (steamUserProfile) {
                        player.steamUserProfile = steamUserProfile;
                    }
                }
            }
        });
    }
}

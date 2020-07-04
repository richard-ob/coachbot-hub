import { Component, OnInit } from '@angular/core';
import { FantasyService } from '../../../shared/services/fantasy.service';
import { FantasyTeam } from '../../../shared/model/fantasy-team.model';
import { ActivatedRoute } from '@angular/router';
import { FantasyPlayer } from '../../../shared/model/fantasy-player.model';
import { FantasyTeamSelection } from '../../../shared/model/fantasy-team-selection.model';
import { PositionGroup } from '../../../shared/model/position-group.enum';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Tournament } from '@pages/hub/shared/model/tournament.model';
import { SwalPortalTargets } from '@sweetalert2/ngx-sweetalert2';
import { TournamentService } from '@pages/hub/shared/services/tournament.service';
import { FantasyTeamSummary, FantasyTeamStatus } from '@pages/hub/shared/model/fantasy-team-summary.model';

@Component({
    selector: 'app-fantasy-team-editor',
    templateUrl: './fantasy-team-editor.component.html',
    styleUrls: ['./fantasy-team-editor.component.scss']
})
export class FantasyTeamEditorComponent implements OnInit {

    fantasyTeamId: number;
    fantasyTeam: FantasyTeam;
    fantasyTeamSummary: FantasyTeamSummary;
    tournament: Tournament;
    positionGroups = PositionGroup;
    fantasyOpen = FantasyTeamStatus.Open;
    teamName: string;
    isLoading = true;
    isUpdating = false;

    constructor(
        private fantasyService: FantasyService,
        private tournamentService: TournamentService,
        private route: ActivatedRoute,
        private snackBar: MatSnackBar,
        public readonly swalTargets: SwalPortalTargets
    ) { }

    ngOnInit() {
        this.route.paramMap.pipe().subscribe(params => {
            this.fantasyTeamId = +params.get('id');
            this.fantasyService.getFantasyTeam(this.fantasyTeamId).subscribe(fantasyTeam => {
                this.fantasyTeam = fantasyTeam;
                this.teamName = fantasyTeam.name;
                this.tournamentService.getTournament(this.fantasyTeam.tournamentId).subscribe(tournament => {
                    this.tournament = tournament;
                    this.fantasyService.getFantasyTeamSummary(this.fantasyTeamId).subscribe(fantasyTeamSummary => {
                        this.fantasyTeamSummary = fantasyTeamSummary;
                        this.isLoading = false;
                    });
                });
            });
        });
    }

    updateTeamName() {
        this.isLoading = true;
        this.fantasyTeam.name = this.teamName;
        this.fantasyService.updateFantasyTeam(this.fantasyTeam).subscribe(() => {
            this.isLoading = false;
        });
    }

    addFantasyTeamSelection(fantasyPlayer: [FantasyPlayer, boolean]) {
        if (this.fantasyTeam.fantasyTeamSelections.some(f => f.fantasyPlayerId === fantasyPlayer[0].id)) {
            this.snackBar.open('Cannot add player as they are already added to your team', 'Dismiss', { duration: 5000 });
        } else if (!this.canAddToGroup(fantasyPlayer[0].positionGroup, fantasyPlayer[1])) {
            this.snackBar.open('Cannot add player as there are no available slots', 'Dismiss', { duration: 5000 });
        } else if (this.getSquadValue() + fantasyPlayer[0].rating > this.tournament.fantasyPointsLimit) {
            this.snackBar.open('Cannot add player as you do not have enough remaining budget', 'Dismiss', { duration: 5000 });
        } else if (this.fantasyTeam.fantasyTeamSelections.filter(f => f.fantasyPlayer.teamId === fantasyPlayer[0].teamId).length === 3) {
            this.snackBar.open(
                `Cannot add player as you already have 3 players from ${fantasyPlayer[0].team.name}`, 'Dismiss', { duration: 5000 }
            );
        } else {
            this.isUpdating = true;
            const selection = new FantasyTeamSelection();
            selection.fantasyPlayer = fantasyPlayer[0];
            selection.fantasyPlayerId = fantasyPlayer[0].id;
            selection.fantasyTeamId = this.fantasyTeamId;
            selection.isFlex = fantasyPlayer[1];
            this.fantasyService.addFantasyTeamSelection(selection).subscribe(() => {
                this.fantasyService.getFantasyTeam(this.fantasyTeamId).subscribe(fantasyTeam => {
                    this.fantasyTeam = fantasyTeam;
                    this.isUpdating = false;
                });
            });
        }
    }

    removeFantasyTeamSelection(fantasyTeamSelection: FantasyTeamSelection) {
        this.isUpdating = true;
        this.fantasyService.removeFantasyTeamSelection(fantasyTeamSelection).subscribe(() => {
            this.fantasyService.getFantasyTeam(this.fantasyTeamId).subscribe(fantasyTeam => {
                this.fantasyTeam = fantasyTeam;
                this.isUpdating = false;
            });
        });
    }

    private canAddToGroup(positionGroup: PositionGroup, isFlex: boolean) {
        if (isFlex && this.fantasyTeam.fantasyTeamSelections.filter(f => f.isFlex).length < 4) {
            return true;
        } else if (isFlex) {
            return false;
        }

        if (positionGroup === PositionGroup.Goalkeeper && this.getPlayerCountForPosition(positionGroup) < 1) {
            return true;
        } else if (positionGroup === PositionGroup.Defence && this.getPlayerCountForPosition(positionGroup) < 3) {
            return true;
        } else if (positionGroup === PositionGroup.Midfield && this.getPlayerCountForPosition(positionGroup) < 1) {
            return true;
        } else if (positionGroup === PositionGroup.Attack && this.getPlayerCountForPosition(positionGroup) < 3) {
            return true;
        }

        return false;
    }

    private getPlayerCountForPosition(positionGroup): number {
        return this.fantasyTeam.fantasyTeamSelections.filter(f => f.fantasyPlayer.positionGroup === positionGroup && !f.isFlex).length;
    }

    getSquadValue(): number {
        return this.fantasyTeam.fantasyTeamSelections.reduce((a, b) => a + b.fantasyPlayer.rating, 0);
    }

    isSquadFull(): boolean {
        return this.fantasyTeam.fantasyTeamSelections.length === 11;
    }

}

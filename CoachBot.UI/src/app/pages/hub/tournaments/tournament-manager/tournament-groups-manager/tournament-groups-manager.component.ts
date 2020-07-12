import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { TournamentService } from '@pages/hub/shared/services/tournament.service';
import { Tournament } from '@pages/hub/shared/model/tournament.model';
import { TournamentGroup } from '@pages/hub/shared/model/tournament-group.model';

@Component({
    selector: 'app-tournament-groups-manager',
    templateUrl: './tournament-groups-manager.component.html'
})
export class TournamentGroupsManagerComponent implements OnInit {

    tournamentId: number;
    tournament: Tournament;
    tournamentGroup: TournamentGroup = new TournamentGroup();
    isSaving = false;
    isLoading = true;

    constructor(
        private tournamentService: TournamentService,
        private route: ActivatedRoute,
        private router: Router
    ) { }

    ngOnInit() {
        this.route.parent.paramMap.pipe().subscribe(params => {
            this.tournamentId = +params.get('id');
            this.loadTournament();
        });
    }

    loadTournament() {
        this.isLoading = true;
        this.tournamentService.getTournament(this.tournamentId).subscribe(tournament => {
            this.tournament = tournament;
            this.isLoading = false;
        });
    }

    createTournamentGroup(tournamentStageId: number) {
        this.isLoading = true;
        this.tournamentGroup.tournamentStageId = tournamentStageId;
        this.tournamentService.createTournamentGroup(this.tournamentGroup).subscribe(() => {
            this.loadTournament();
            this.tournamentGroup = new TournamentGroup();
        });
    }

    deleteTournamentGroup(tournamentGroupId: number) {
        this.isLoading = true;
        this.tournamentService.deleteTournamentGroup(tournamentGroupId).subscribe(() => {
            this.loadTournament();
        });
    }

    removeTournamentGroupTeam(teamId: number, tournamentGroupId: number) {
        this.tournamentService.removeTournamentGroupTeam(teamId, tournamentGroupId).subscribe(() => {
            this.loadTournament();
        });
    }

    editMatch(matchId: number) {
        this.router.navigate(['/match-editor/', matchId]);
    }

}

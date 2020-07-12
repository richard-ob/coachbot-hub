import { Component, OnInit } from '@angular/core';
import { TournamentService } from '../../shared/services/tournament.service';
import { Tournament } from '../../shared/model/tournament.model';
import { ActivatedRoute, Router } from '@angular/router';
import { TournamentGroup } from '../../shared/model/tournament-group.model';
import { TournamentStaff } from '../../shared/model/tournament-staff.model';
import { TournamentStaffRole } from '../../shared/model/tournament-staff-role.model';
import { FantasyService } from '@pages/hub/shared/services/fantasy.service';

@Component({
    selector: 'app-tournament-manager',
    templateUrl: './tournament-manager.component.html'
})
export class TournamentManagerComponent implements OnInit {

    tournamentId: number;
    tournament: Tournament;
    tournamentGroup: TournamentGroup = new TournamentGroup();
    tournamentStaff = new TournamentStaff();
    tournamentRole = TournamentStaffRole;
    isSaving = false;
    isLoading = true;

    constructor(
        private tournamentService: TournamentService,
        private fantasyService: FantasyService,
        private route: ActivatedRoute,
        private router: Router
    ) { }

    ngOnInit() {
        this.route.paramMap.pipe().subscribe(params => {
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

}

import { Component, OnInit } from '@angular/core';
import { TournamentService } from '../../shared/services/tournament.service';
import { TournamentEdition } from '../../shared/model/tournament-edition.model';
import { ActivatedRoute } from '@angular/router';

@Component({
    selector: 'app-tournament-overview',
    templateUrl: './tournament-overview.component.html'
})
export class TournamentOverviewComponent implements OnInit {

    tournamenEditionId: number;
    tournamentEdition: TournamentEdition;
    isLoading = true;

    constructor(private tournamentService: TournamentService, private route: ActivatedRoute) { }

    ngOnInit() {
        this.route.paramMap.pipe().subscribe(params => {
            this.tournamenEditionId = +params.get('id');
            this.tournamentService.getTournamentEdition(this.tournamenEditionId).subscribe(tournamentEdition => {
                this.tournamentEdition = tournamentEdition;
                this.isLoading = false;
            });
        });
    }

}

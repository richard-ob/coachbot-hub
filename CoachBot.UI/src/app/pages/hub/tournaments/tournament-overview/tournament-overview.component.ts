import { Component, OnInit } from '@angular/core';
import { TournamentService } from '../../shared/services/tournament.service';
import { Tournament } from '../../shared/model/tournament.model';
import { ActivatedRoute } from '@angular/router';
import ColourUtils from '@shared/utilities/colour-utilities';

@Component({
    selector: 'app-tournament-overview',
    templateUrl: './tournament-overview.component.html',
    styleUrls: ['./tournament-overview.component.scss']
})
export class TournamentOverviewComponent implements OnInit {

    tournamentId: number;
    tournament: Tournament;
    isLoading = true;

    constructor(private tournamentService: TournamentService, private route: ActivatedRoute) { }

    ngOnInit() {
        this.route.paramMap.pipe().subscribe(params => {
            this.tournamentId = +params.get('id');
            this.tournamentService.getTournament(this.tournamentId).subscribe(tournament => {
                this.tournament = tournament;
                this.isLoading = false;
            });
        });
    }

    generateGradient(colour: string) {
        const gradientSrc =
            'linear-gradient(90deg,' + ColourUtils.hexToRgbA(colour, 0.6) + ',' + ColourUtils.hexToRgbA(colour, 0.3) + ')';

        return gradientSrc;
    }

}

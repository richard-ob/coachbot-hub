import { Component, OnInit, Input } from '@angular/core';
import { TournamentService } from '@pages/hub/shared/services/tournament.service';
import { TournamentEdition } from '@pages/hub/shared/model/tournament-edition.model';
import { TournamentGroupMatch } from '@pages/hub/shared/model/tournament-group-match.model';

@Component({
    selector: 'app-tournament-standings-knockout',
    templateUrl: './tournament-standings-knockout.component.html',
    styleUrls: ['./tournament-standings-knockout.component.scss']
})
export class TournamentStandingsKnockoutComponent implements OnInit {

    @Input() tournamentEditionId: number;
    tournamentEdition: TournamentEdition;
    matches: TournamentGroupMatch;
    isLoading = true;

    constructor(private tournamentService: TournamentService) {
    }

    ngOnInit() {
        this.loadTournamentEdition();
    }

    loadTournamentEdition() {
        this.isLoading = true;
        this.tournamentService.getTournamentEdition(this.tournamentEditionId).subscribe(tournamentEdition => {
            this.tournamentEdition = tournamentEdition;
            this.isLoading = false;
        });
    }

    nextRoundIncludesByedTeams(phaseIndex: number) {
        console.log(phaseIndex);
        const phases = this.tournamentEdition.tournamentStages[0].tournamentPhases;
        const currentPhase = phases[phaseIndex];
        const nextPhase = phases.length > phaseIndex + 1 ? phases[phaseIndex + 1] : null;
        if (nextPhase) {
            const matches = this.tournamentEdition.tournamentStages[0].tournamentGroups[0].tournamentGroupMatches;
            const currentPhaseMatches = matches.filter(m => m.tournamentPhaseId === currentPhase.id);
            const nextPhaseMatches = matches.filter(m => m.tournamentPhaseId === nextPhase.id);
            console.log(currentPhaseMatches);
            console.log(nextPhaseMatches);
            if (currentPhaseMatches && nextPhaseMatches && currentPhaseMatches.length === nextPhaseMatches.length) {
                return true;
            }
        }
        return false;
    }

}

import { Component, OnInit, Input, ViewEncapsulation } from '@angular/core';
import { TournamentService } from '../../../shared/services/tournament.service';
import { TournamentEditionMatchDaySlot, TournamentMatchDay } from '@pages/hub/shared/model/tournament-edition-match-day-slot.model';

@Component({
    selector: 'app-tournament-match-day-slot-manager',
    templateUrl: './tournament-match-day-slot-manager.component.html',
    styleUrls: ['./tournament-match-day-slot-manager.component.scss'],
    encapsulation: ViewEncapsulation.None
})
export class TournamentMatchDaySlotManagerComponent implements OnInit {

    @Input() tournamentEditionId: number;
    matchDaySlots: TournamentEditionMatchDaySlot[];
    matchDaySlot: TournamentEditionMatchDaySlot;
    matchDays = TournamentMatchDay;
    isLoading = true;

    constructor(private tournamentService: TournamentService) { }

    ngOnInit() {
        this.initialiseMatchDaySlot();
        this.loadMatchDaySlots();
    }

    loadMatchDaySlots() {
        this.tournamentService.getTournamentEditionMatchDaySlots(this.tournamentEditionId).subscribe(matchDaySlots => {
            this.matchDaySlots = matchDaySlots;
            this.isLoading = false;
        });
    }

    createMatchDaySlot() {
        this.isLoading = true;
        this.tournamentService.createTournamentEditionMatchDaySlot(this.matchDaySlot).subscribe(() => {
            this.initialiseMatchDaySlot();
            this.loadMatchDaySlots();
        });
    }

    deleteMatchDaySlot(matchDaySlotId: number) {
        this.isLoading = true;
        this.tournamentService.deleteTournamentEditionMatchDaySlot(this.tournamentEditionId, matchDaySlotId).subscribe(() => {
            this.initialiseMatchDaySlot();
            this.loadMatchDaySlots();
        });
    }

    initialiseMatchDaySlot() {
        this.matchDaySlot = {
            matchDay: 1,
            matchTime: new Date(),
            tournamentEditionId: this.tournamentEditionId
        };
    }

}

import { Component, OnInit, Input, ViewEncapsulation } from '@angular/core';
import { TournamentService } from '../../../shared/services/tournament.service';
import { TournamentMatchDay, TournamentMatchDaySlot } from '@pages/hub/shared/model/tournament-match-day-slot.model';
import { ActivatedRoute } from '@angular/router';

@Component({
    selector: 'app-tournament-match-day-slot-manager',
    templateUrl: './tournament-match-day-slot-manager.component.html',
    styleUrls: ['./tournament-match-day-slot-manager.component.scss'],
    encapsulation: ViewEncapsulation.None
})
export class TournamentMatchDaySlotManagerComponent implements OnInit {

    tournamentId: number;
    matchDaySlots: TournamentMatchDaySlot[];
    matchDaySlot: TournamentMatchDaySlot;
    matchDays = TournamentMatchDay;
    isLoading = true;

    constructor(private tournamentService: TournamentService, private route: ActivatedRoute) { }

    ngOnInit() {
        this.route.parent.paramMap.pipe().subscribe(params => {
            this.tournamentId = +params.get('id');
            this.initialiseMatchDaySlot();
            this.loadMatchDaySlots();
        });
    }

    loadMatchDaySlots() {
        this.tournamentService.getTournamentMatchDaySlots(this.tournamentId).subscribe(matchDaySlots => {
            this.matchDaySlots = matchDaySlots;
            this.isLoading = false;
        });
    }

    createMatchDaySlot() {
        this.isLoading = true;
        this.tournamentService.createTournamentMatchDaySlot(this.matchDaySlot).subscribe(() => {
            this.initialiseMatchDaySlot();
            this.loadMatchDaySlots();
        });
    }

    deleteMatchDaySlot(matchDaySlotId: number) {
        this.isLoading = true;
        this.tournamentService.deleteTournamentMatchDaySlot(this.tournamentId, matchDaySlotId).subscribe(() => {
            this.initialiseMatchDaySlot();
            this.loadMatchDaySlots();
        });
    }

    initialiseMatchDaySlot() {
        this.matchDaySlot = {
            matchDay: 1,
            matchTime: new Date(),
            tournamentId: this.tournamentId
        };
    }

}

import { Component, OnInit } from '@angular/core';
import { MatchService } from '../shared/services/match.service';
import { Match } from '../shared/model/match.model';
@Component({
    selector: 'app-recent-matches',
    templateUrl: './recent-matches.component.html',
    styleUrls: ['./recent-matches.component.scss']
})
export class RecentMatchesComponent implements OnInit {

    matches: Match[];

    constructor(private matchService: MatchService) {

    }

    ngOnInit() {
        this.matchService.getMatches(1, 1).subscribe(matches => {
            this.matches = matches;
        });
    }

}

import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { TeamService } from '../../shared/services/team.service';
import { TeamStatistics } from '../../shared/model/team-statistics.model';

@Component({
    selector: 'app-team-profile-tournaments',
    templateUrl: './team-profile-tournaments.component.html'
})
export class TeamProfileTournamentsComponent implements OnInit {

    teamId: number;
    teamStatistics: TeamStatistics;
    isLoading = true;

    constructor(private route: ActivatedRoute, private teamService: TeamService) { }

    ngOnInit() {
        this.route.parent.paramMap.pipe().subscribe(params => {
            this.teamId = +params.get('id');
        });
    }

}

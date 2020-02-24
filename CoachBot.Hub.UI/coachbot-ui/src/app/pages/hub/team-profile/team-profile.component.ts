import { Component, OnInit } from '@angular/core';
import { TeamService } from '../shared/services/team.service';
import { ActivatedRoute } from '@angular/router';
import { Team } from '../shared/model/team.model';

@Component({
    selector: 'app-team-profile',
    templateUrl: './team-profile.component.html',
    styleUrls: ['./team-profile.component.scss']
})
export class TeamProfileComponent implements OnInit {

    team: Team;
    isLoading = true;

    constructor(private teamService: TeamService, private route: ActivatedRoute) { }

    ngOnInit() {
        this.route.paramMap.pipe().subscribe(params => {
            this.teamService.getTeam(+params.get('id')).subscribe(team => {
                this.team = team;
                this.isLoading = false;
            });
        });
    }

}

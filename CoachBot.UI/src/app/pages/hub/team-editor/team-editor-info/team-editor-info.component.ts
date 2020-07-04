import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { TeamService } from '@pages/hub/shared/services/team.service';
import { Team } from '@pages/hub/shared/model/team.model';

@Component({
    selector: 'app-team-editor-info',
    templateUrl: './team-editor-info.component.html'
})
export class TeamEditorInfoComponent implements OnInit {

    team: Team;
    isLoading = true;

    constructor(private teamService: TeamService, private route: ActivatedRoute) { }

    ngOnInit() {
        this.route.parent.paramMap.pipe().subscribe(params => {
            const teamId = +params.get('id');
            this.teamService.getTeam(teamId).subscribe(team => {
                this.team = team;
                this.isLoading = false;
            });
        });
    }

}

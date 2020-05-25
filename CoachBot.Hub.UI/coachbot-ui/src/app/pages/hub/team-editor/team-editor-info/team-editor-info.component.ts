import { Component, Input, OnInit } from '@angular/core';
import { Team } from '@pages/hub/shared/model/team.model';
import { TeamService } from '@pages/hub/shared/services/team.service';
import { Region } from '@pages/hub/shared/model/region.model';
import { RegionService } from '@pages/hub/shared/services/region.service';
import { TeamType } from '@pages/hub/shared/model/team-type.enum';
import { Router } from '@angular/router';

@Component({
    selector: 'app-team-editor-info',
    templateUrl: './team-editor-info.component.html'
})
export class TeamEditorInfoComponent implements OnInit {

    @Input() team: Team;
    regions: Region[];
    teamTypes = TeamType;
    isSaving = false;
    isLoading = true;

    constructor(private teamService: TeamService, private regionService: RegionService, private router: Router) {

    }

    ngOnInit() {
        this.regionService.getRegions().subscribe(regions => {
            this.regions = regions;
            this.isLoading = false;
        });
    }

    saveTeamProfile() {
        this.isSaving = true;
        if (!this.team.id) {
            this.teamService.createTeam(this.team).subscribe(() => {
                this.router.navigate(['team-editor-list']);
            });
        } else {
            this.teamService.updateTeam(this.team).subscribe(() => {
                this.teamService.getTeam(this.team.id).subscribe(team => {
                    this.team = team;
                    this.isSaving = false;
                });
            });
        }
    }

    updateBadgeImageId(assetImageId: number) {
        this.team.badgeImageId = assetImageId;
    }
}

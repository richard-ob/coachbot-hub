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

    generateGradient(colour: string) {
        // return 'background-image: linear-gradient(90deg,' + this.hexToRgbA(colour, 0.8) + ',' + this.hexToRgbA(colour, 0.3) + ');';
    }

    private hexToRgbA(hex: string, opacity: number = 1) {
        let colour;
        if (/^#([A-Fa-f0-9]{3}){1,2}$/.test(hex)) {
            colour = hex.substring(1).split('');
            if (colour.length === 3) {
                colour = [colour[0], colour[0], colour[1], colour[1], colour[2], colour[2]];
            }
            colour = '0x' + colour.join('');
            // tslint:disable-next-line:no-bitwise
            return 'rgba(' + [(colour >> 16) & 255, (colour >> 8) & 255, colour & 255].join(',') + ', ' + opacity + ')';
        }
    }
}

import { TestBed, async, ComponentFixture, ComponentFixtureAutoDetect, } from '@angular/core/testing';
import { ProfileComponent } from './profile.component';
import { FormsModule } from '@angular/forms';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { of } from 'rxjs';
import { UserService } from '../shared/services/user.service';
import { LeaderboardService } from '../shared/services/leaderboard.service';
import { User } from '../model/user';

describe('ProfileComponent', () => {

    let component: ProfileComponent;
    let fixture: ComponentFixture<ProfileComponent>;
    let getUserSpy: jasmine.Spy;
    let getUserStatisticsSpy: jasmine.Spy;
    let getPlayerLeaderboardSpy: jasmine.Spy;
    let getLeaderboardForPlayerSpy: jasmine.Spy;

    beforeEach(() => {

        const expectedUser: User = { id: 1, name: 'A', IsAdministrator: false, discordUserIdString: '1', channels: [] };
        const expectedUserStatistics = { appearances: 100, firstAppearance: new Date(), lastAppearance: new Date(), rank: 1 };

        const userService = jasmine.createSpyObj('UserService', ['getUser', 'getUserStatistics']);
        getUserSpy = userService.getUser.and.returnValue(of(expectedUser));
        getUserStatisticsSpy = userService.getUserStatistics.and.returnValue(of(expectedUserStatistics));

        const leaderboardService = jasmine.createSpyObj('LeaderboardService', ['getLeaderboardForPlayer', 'getPlayerLeaderboard']);
        getPlayerLeaderboardSpy = leaderboardService.getPlayerLeaderboard.and.returnValue(of([{ key: 'player', value: 1 }]));
        getLeaderboardForPlayerSpy = leaderboardService.getLeaderboardForPlayer.and.returnValue(of([{ key: 'player', value: 1 }]));

        TestBed.configureTestingModule({
            declarations: [ProfileComponent],
            providers: [
                { provide: UserService, useValue: userService },
                { provide: LeaderboardService, useValue: leaderboardService },
                { provide: ComponentFixtureAutoDetect, useValue: true }
            ],
            imports: [
                FormsModule,
                HttpClientTestingModule
            ]
        });

        fixture = TestBed.createComponent(ProfileComponent);
        component = fixture.componentInstance;
    });

    it('should create the profile component', async(() => {
        expect(component).toBeTruthy();
    }));

    it('should call getUser in userService at least once', async(() => {
        expect(getUserSpy).toHaveBeenCalled();
    }));

    it('should call getUserStatistics in userService at least once', async(() => {
        expect(getUserStatisticsSpy).toHaveBeenCalled();
    }));

    it('should call getPlayerLeaderboard in userService at least once', async(() => {
        expect(getPlayerLeaderboardSpy).toHaveBeenCalled();
    }));

    it('should call getLeaderboardForPlayer in userService at least once', async(() => {
        expect(getLeaderboardForPlayerSpy).toHaveBeenCalled();
    }));
});

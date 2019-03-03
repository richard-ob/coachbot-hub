import { asyncData } from '../testing/async-observable-helper';
import { LeaderboardService } from './leaderboard.service';
import { Player } from '../../model/player';

describe('LeaderboardService', () => {
    let httpClientSpy: { get: jasmine.Spy };
    let leaderboardService: LeaderboardService;

    beforeEach(() => {
        httpClientSpy = jasmine.createSpyObj('HttpClient', ['get']);
        leaderboardService = new LeaderboardService(<any>httpClientSpy);
    });

    it('#getPlayerLeaderboard should return array of players & call httpClient only once', () => {
        const expectedLeaderboard: Player[] = [];
        httpClientSpy.get.and.returnValue(asyncData(expectedLeaderboard));

        leaderboardService.getPlayerLeaderboard()
            .subscribe(leaderboard => expect(leaderboard).toEqual(expectedLeaderboard, 'expected leaderboard'), fail);

        expect(httpClientSpy.get.calls.count()).toBe(1, 'one call');
    });
});

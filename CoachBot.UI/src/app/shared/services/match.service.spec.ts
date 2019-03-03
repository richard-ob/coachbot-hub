import { asyncData } from '../testing/async-observable-helper';
import { MatchService } from './match.service';
import { Match } from '../../model/match';

describe('MatchService', () => {
    let httpClientSpy: { get: jasmine.Spy };
    let matchService: MatchService;

    beforeEach(() => {
        httpClientSpy = jasmine.createSpyObj('HttpClient', ['get']);
        matchService = new MatchService(<any>httpClientSpy);
    });

    it('#getMatchHistory should return array for match objects & call httpClient only once', () => {
        const expectedMatchHistory: Match[] = [
            {
                channelId: 1234567890, channelName: 'Channel #1', matchDate: new Date(),
                team1Name: 'Team #1', team2Name: 'Team #2', players: []
            },
            {
                channelId: 1234567890, channelName: 'Channel #1', matchDate: new Date(),
                team1Name: 'Team #1', team2Name: 'Team #3', players: []
            }
        ];
        httpClientSpy.get.and.returnValue(asyncData(expectedMatchHistory));

        matchService.getMatchHistory('1234567890')
            .subscribe(matchHistory => expect(matchHistory).toEqual(expectedMatchHistory, 'expected match history'), fail);

        expect(httpClientSpy.get.calls.count()).toBe(1, 'one call');
    });
});

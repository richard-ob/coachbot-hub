import { UserService } from './user.service';
import { User } from '../../model/user';
import { asyncData } from '../testing/async-observable-helper';
import { Player } from '../../model/player';

describe('UserService', () => {
    let httpClientSpy: { get: jasmine.Spy };
    let userService: UserService;

    beforeEach(() => {
        httpClientSpy = jasmine.createSpyObj('HttpClient', ['get']);
        userService = new UserService(<any>httpClientSpy);
    });

    it('#getUser should return current user & call httpClient only once', () => {
        const expectedUser: User = { id: 1, name: 'A', IsAdministrator: false, discordUserIdString: '1', channels: [] };
        httpClientSpy.get.and.returnValue(asyncData(expectedUser));

        userService.getUser().subscribe(user => expect(user).toEqual(expectedUser, 'expected user'), fail);

        expect(httpClientSpy.get.calls.count()).toBe(1, 'one call');
    });

    it('#getUserStatistics should return array of players & call httpClient only once', () => {
        const expectedPlayers: Player[] = [
            { name: 'Test Player', discordUserId: 1, discordUserMention: '@1', position: null },
            { name: 'Test Player 2', discordUserId: 2, discordUserMention: '@2', position: null }
        ];
        httpClientSpy.get.and.returnValue(asyncData(expectedPlayers));

        userService.getUserStatistics().subscribe(players => expect(players).toEqual(expectedPlayers, 'expected players'), fail);

        expect(httpClientSpy.get.calls.count()).toBe(1, 'one call');
    });
});

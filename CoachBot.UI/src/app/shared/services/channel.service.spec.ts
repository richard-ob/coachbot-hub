import { asyncData } from '../testing/async-observable-helper';
import { ChannelService } from './channel.service';
import { Channel } from '../../model/channel';

describe('ChannelService', () => {
    let httpClientSpy: { get: jasmine.Spy };
    let channelService: ChannelService;

    beforeEach(() => {
        httpClientSpy = jasmine.createSpyObj('HttpClient', ['get']);
        channelService = new ChannelService(<any>httpClientSpy);
    });

    it('#getChannels should return channel list & call httpClient only once', () => {
        const expectedChannels: Channel[] = [
            {
                id: 111111,
                idString: '11111',
                name: 'Channel #1',
                guildName: 'Guild #1',
                isMixChannel: false,
                classicLineup: false,
                positions:
                    [
                        { positionName: '#1' },
                        { positionName: '#2' }
                    ],
                team1: {
                    name: 'Team #1',
                    kitEmote: ':emote:',
                    color: '#fff',
                    isMix: false,
                    badgeEmote: null,
                    players: []
                },
                team2: {
                    name: 'Team #2',
                    kitEmote: ':emote:',
                    color: '#000',
                    isMix: false,
                    badgeEmote: null,
                    players: []
                },
                emotes: [],
                formation: 1,
                region: { regionId: 1, regionName: 'Europe', serverCount: 1 }
            }
        ];
        httpClientSpy.get.and.returnValue(asyncData(expectedChannels));

        channelService.getChannels().subscribe(channels => expect(channels).toEqual(expectedChannels, 'expected channels'), fail);

        expect(httpClientSpy.get.calls.count()).toBe(1, 'one call');
    });
});

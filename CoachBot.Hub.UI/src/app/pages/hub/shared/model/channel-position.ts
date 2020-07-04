import { Channel } from './channel.model';
import { Position } from './position';

export class ChannelPosition {
    channelId: number;
    channel?: Channel;
    positionId: number;
    position?: Position;
    ordinal: number;
}

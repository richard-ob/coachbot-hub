import { Pipe, PipeTransform } from '@angular/core';
import { PositionGroup } from '../model/position-group.enum';

@Pipe({ name: 'positionGroup' })
export class PositionGroupPipe implements PipeTransform {
    transform(positionGroup: PositionGroup): string {
        switch (positionGroup) {
            case PositionGroup.Attack:
                return 'ATK';
            case PositionGroup.Defence:
                return 'DEF';
            case PositionGroup.Goalkeeper:
                return 'GK';
            case PositionGroup.Midfield:
                return 'MID';
            default:
                return '';
        }
    }
}

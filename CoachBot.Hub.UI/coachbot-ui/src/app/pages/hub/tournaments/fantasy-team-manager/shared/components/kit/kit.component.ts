import { Component, Input } from '@angular/core';
import { DomSanitizer } from '@angular/platform-browser';

@Component({
    selector: 'app-kit',
    templateUrl: './kit.component.html',
    styleUrls: ['./kit.component.scss']
})
export class KitComponent {

    @Input() color = '#124364';
    @Input() text: string;
    @Input() badge: string;
    @Input() icon: string;

    constructor() { }

}

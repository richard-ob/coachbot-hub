import { Component, Input } from '@angular/core';
import { DomSanitizer } from '@angular/platform-browser';

@Component({
    selector: 'app-kit',
    templateUrl: './kit.component.html'
})
export class KitComponent {

    @Input() color = '#124364';
    @Input() text: string;


    constructor(private sanitizer: DomSanitizer) { }

    svgKit() {
        return this.sanitizer.bypassSecurityTrustStyle(`fill:${this.color};fill-opacity:1;fill-rule:nonzero;stroke:none`);
    }
}

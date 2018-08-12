import { Component, Input } from '@angular/core';
import { DomSanitizer } from '@angular/platform-browser';

@Component({
    selector: 'app-kit',
    templateUrl: './kit.component.html'
})
export class KitComponent {

    @Input() color: string;
    @Input() text: string;
    svgKitColor: string;


    constructor(private _sanitizer: DomSanitizer) { }

    svgKit() {
        return this._sanitizer.bypassSecurityTrustStyle(`fill:${this.color};fill-opacity:1;fill-rule:nonzero;stroke:none`);
    }
}

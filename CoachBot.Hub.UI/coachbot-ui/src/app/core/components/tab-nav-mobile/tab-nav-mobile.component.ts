import { Directive, ElementRef, Component } from '@angular/core';

@Component({
    selector: '[appTabNavMobile]',
    template: '<ng-content></ng-content>'
})
export class TabNavMobileComponent {

    constructor(private elementRef: ElementRef) { }

    toggle() {
        this.elementRef.nativeElement.classList.toggle('content-filter__list--expanded');
    }
}

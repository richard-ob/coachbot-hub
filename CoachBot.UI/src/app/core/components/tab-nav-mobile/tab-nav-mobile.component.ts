import { Directive, ElementRef, Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
    selector: '[appTabNavMobile]',
    template: '<ng-content></ng-content>'
})
export class TabNavMobileComponent {

    constructor(private elementRef: ElementRef, private router: Router) {
        this.router.events.subscribe(event => {
            this.elementRef.nativeElement.classList.remove('content-filter__list--expanded');
        });
    }

    toggle() {
        this.elementRef.nativeElement.classList.toggle('content-filter__list--expanded');
        if (this.elementRef.nativeElement.classList.contains('content-filter__list--expanded')) {
            window.scrollTo(0, 0);
        }
    }
}

import { Component, OnInit, ChangeDetectionStrategy } from '@angular/core';

@Component({
    selector: 'app-server-manual',
    templateUrl: './server-manual.component.html',
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class ServerManualComponent implements OnInit {

    constructor() { }

    ngOnInit() {
    }

}

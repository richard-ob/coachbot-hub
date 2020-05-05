import { Component, Input, Output, HostBinding, HostListener, OnInit, EventEmitter, ElementRef } from '@angular/core';
import { ColorPickerControl, Color } from '@iplab/ngx-color-picker';

@Component({
    selector: 'app-chrome-colour-picker',
    template: `
    <chrome-picker *ngIf="isVisible" [control]="colorControl" style="position: absolute; z-index: 100; left: 3rem; margin-top: -26px;">
    </chrome-picker>
    <div *ngIf="isVisible" (click)="closeColorPicker()" style="position: fixed; width: 100%; height: 100%; z-index: 99; top: 0; left: 0;">
    </div>
    `
})
export class ChromeColourPickerComponent implements OnInit {

    public colorControl = new ColorPickerControl();

    public isVisible = false;

    @Input() public set color(color: string) {
        this.colorControl.setValueFrom(color);
    }
    @Output() public colorChange: EventEmitter<string> = new EventEmitter();

    @HostBinding('style.background-color')
    public get background(): string {
        return this.colorControl.value.toHexString();
    }

    constructor(private elementRef: ElementRef) { }

    public ngOnInit() {
        this.colorControl.valueChanges.subscribe((value: Color) => this.colorChange.emit(value.toHexString()));
    }

    public closeColorPicker() {
        this.isVisible = false;
    }

    public showColorPicker() {
        this.isVisible = true;
    }
}

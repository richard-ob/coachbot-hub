import { NgModule } from '@angular/core';
import { ChromeColourPickerComponent } from './chrome-colour-picker.component';
import { ColorPickerModule } from '@iplab/ngx-color-picker';
import { CommonModule } from '@angular/common';

@NgModule({
    declarations: [
        ChromeColourPickerComponent
    ],
    exports: [
        ChromeColourPickerComponent
    ],
    imports: [
        ColorPickerModule,
        CommonModule
    ]
})
export class ChromeColourPickerModule { }

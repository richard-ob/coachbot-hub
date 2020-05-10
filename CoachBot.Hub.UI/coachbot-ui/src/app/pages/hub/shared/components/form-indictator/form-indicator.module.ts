import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SpinnerModule } from 'src/app/core/components/spinner/spinner.module';
import { FormIndicatorComponent } from './form-indicator.component';

@NgModule({
    declarations: [
        FormIndicatorComponent
    ],
    imports: [
        CommonModule,
        SpinnerModule
    ],
    exports: [
        FormIndicatorComponent
    ]
})
export class FormIndicatorModule { }

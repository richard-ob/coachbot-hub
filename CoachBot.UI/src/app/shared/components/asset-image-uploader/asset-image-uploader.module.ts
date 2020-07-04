import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { SpinnerModule } from 'src/app/core/components/spinner/spinner.module';
import { AssetImageUploaderComponent } from './asset-image-uploader.component';

@NgModule({
    declarations: [
        AssetImageUploaderComponent
    ],
    exports: [
        AssetImageUploaderComponent
    ],
    imports: [
        CommonModule,
        FormsModule,
        SpinnerModule
    ]
})
export class AssetImageUploaderModule { }

import { Component, EventEmitter, Output, Input, OnInit } from '@angular/core';
import { AssetImageService } from '../../services/asset-image.service';
import { AssetImage } from '../../models/asset-image.model';

@Component({
    selector: 'app-asset-image-uploader',
    templateUrl: './asset-image-uploader.component.html'
})
export class AssetImageUploaderComponent implements OnInit {

    @Output() imageUploaded = new EventEmitter<number>();
    @Input() currentAssetImageId: number;
    file: AssetImage;
    currentAssetImage: string;
    invalidImage = false;
    uploadSuccessful = false;
    isUploading = false;
    hasUploaded = true;
    PNG_BASE64_REGEX = new RegExp('^data:image//(?:png)(?:;charset=utf-8)?;base64,(?:[A-Za-z0-9]|[+//])+={0,2}');

    constructor(private assetImageService: AssetImageService) { }

    ngOnInit() {
        if (this.currentAssetImageId && this.currentAssetImageId > 0) {
            this.assetImageService.getAssetImage(this.currentAssetImageId).subscribe(assetImage => {
                this.currentAssetImage = assetImage.base64EncodedImage;
            });
        }
    }

    fileSelected(event: any) {
        const file = event.target.files[0];
        const fileReader = new FileReader();
        this.invalidImage = false;
        this.uploadSuccessful = false;
        fileReader.addEventListener('load', () => {
            this.file = {
                base64EncodedImage: fileReader.result,
                fileName: file.name
            };
            if (this.PNG_BASE64_REGEX.test(this.file.base64EncodedImage)) {
                this.invalidImage = true;
            }
        });
        fileReader.readAsDataURL(file);
        this.hasUploaded = false;
    }

    uploadFile() {
        this.uploadSuccessful = false;
        if (!this.invalidImage) {
            this.isUploading = true;
            this.assetImageService.createAssetImage(this.file).subscribe(assetImageId => {
                this.imageUploaded.emit(assetImageId);
                this.isUploading = false;
                this.uploadSuccessful = true;
                this.hasUploaded = true;
            },
                error => {
                    this.invalidImage = true;
                    this.isUploading = false;
                    this.hasUploaded = false;
                }
            );
        }
    }

}

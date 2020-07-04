
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { Injectable } from '@angular/core';
import { AssetImage } from '../models/asset-image.model';

@Injectable({
    providedIn: 'root'
})
export class AssetImageService {

    constructor(private http: HttpClient) { }

    getAssetImage(id: number): Observable<AssetImage> {
        return this.http.get<AssetImage>(`${environment.apiUrl}/api/asset-images/${id}`);
    }

    createAssetImage(assetImage: AssetImage): Observable<number> {
        return this.http.post<number>(`${environment.apiUrl}/api/asset-images`, assetImage);
    }

}

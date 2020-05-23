import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable({
    providedIn: 'root'
})
export class UserPreferenceService {

    userPreferences: UserPreference[] = [];
    userPreferenceDefaults: UserPreference[] = [
        {
            type: UserPreferenceType.Region,
            value: 2
        }
    ];

    constructor(private http: HttpClient) {
        this.userPreferences = JSON.parse(localStorage.getItem('userPreferences')) || this.userPreferenceDefaults;
    }

    setUserPreference(userPreferenceType: UserPreferenceType, value: any) {
        const userPreference = this.userPreferences.find(u => u.type === userPreferenceType);
        userPreference.value = value;
        localStorage.setItem('userPreferences', JSON.stringify(this.userPreferences));
        location.reload();
    }

    getUserPreference(userPreferenceType: UserPreferenceType) {
        return this.userPreferences.find(p => p.type === userPreferenceType).value;
    }

}

export class UserPreference {
    type: UserPreferenceType;
    value: any;
}

export enum UserPreferenceType {
    Region
}

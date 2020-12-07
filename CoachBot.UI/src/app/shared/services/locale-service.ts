import { getBrowserLang } from '@locl/core';
import { Injectable } from '@angular/core';
import { Language } from './language.model';

@Injectable({ providedIn: 'root' })
export class LocaleService {

    private readonly LANGUAGE_LOCAL_STORAGE_KEY = 'language';
    private readonly LANGUAGE_FILE_PATH = '/assets/locales/';
    private readonly LANGUAGE_FILE_EXTENSION = '.json';
    private readonly DEFAULT_LANGUAGE = 'en';
    private readonly LANGUAGES: Language[] = [
        { name: 'English', code: 'en', flag: 'gb' },
        /*{ name: 'Portuguese', code: 'pt', flag: 'pt' },
        { name: 'Spanish', code: 'es', flag: 'es' },*/
        { name: 'Korean', code: 'ko', flag: 'kr' },
        /*{ name: 'Portuguese (PT)', code: 'pt', flag: 'pt' },*/
        { name: 'Polish', code: 'pl', flag: 'pl' },
        /*{ name: 'Czech', code: 'cs', flag: 'cz' },*/
        //{ name: 'French', code: 'fr', flag: 'fr' },
    ];

    public getLanguages(): Language[] {
        return this.LANGUAGES;
    }

    public getLanguage(): Language {
        const currentLanguageCode = localStorage.getItem(this.LANGUAGE_LOCAL_STORAGE_KEY) || this.getBrowserLanguage() || this.DEFAULT_LANGUAGE;        
        const currentLanguage = this.LANGUAGES.find(language => currentLanguageCode === language.code);
        
        if (currentLanguage) {
            return currentLanguage;
        }

        return this.LANGUAGES[0];
    }

    public setLanguage(isoCode: string): void {
        localStorage.setItem(this.LANGUAGE_LOCAL_STORAGE_KEY, isoCode);        
        location.reload();
    }

    public getLanguageFilePath(): string {
        const currentLanguage = this.getLanguage();
        return this.LANGUAGE_FILE_PATH + currentLanguage.code + '/' + currentLanguage.code + this.LANGUAGE_FILE_EXTENSION;
    }

    private getBrowserLanguage(): string {
        return getBrowserLang().toLowerCase();
    }
}

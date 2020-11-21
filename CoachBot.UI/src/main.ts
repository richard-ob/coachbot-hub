import 'hammerjs';
import { enableProdMode } from '@angular/core';
import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';

import { AppModule } from './app/app.module';
import { environment } from './environments/environment';
import { getTranslations, ParsedTranslationBundle } from '@locl/core';
import { LocaleService } from '@shared/services/locale-service';

if (environment.production) {
  enableProdMode();
}

const localeService = new LocaleService();
getTranslations(localeService.getLanguageFilePath())
    .then((data: ParsedTranslationBundle) => {
        platformBrowserDynamic()
            .bootstrapModule(AppModule)
            .catch(err => console.error(err));
    })
    .catch(() => console.error(`Language file could not be found for ${localeService.getLanguage().name}`));
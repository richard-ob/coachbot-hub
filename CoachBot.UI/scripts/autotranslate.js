'use strict';

const fs = require('fs');
const { readdirSync } = require('fs')

const localesPath = 'src/assets/locales/';
const locales = readdirSync(localesPath, { withFileTypes: true })
    .filter(dirent => dirent.isDirectory() && !dirent.name.startsWith('en'))
    .map(dirent => dirent.name)

for (const locale of locales) {
    convertFromAutoTranslateToLoclFileFormat(locale);
}

function convertFromAutoTranslateToLoclFileFormat(localeCode) {
    const sourceFile = localesPath + localeCode + '/en.json';
    const rawdata = fs.readFileSync(sourceFile);
    const translationFile = JSON.parse(rawdata);

    const translationFileOutput = {
        locale: localeCode,
        translations: flatten(translationFile.translations)
    };

    const data = JSON.stringify(translationFileOutput);
    const targetFile = localesPath + localeCode + '/' + localeCode + '.json';
    fs.writeFileSync(targetFile, data);
    fs.unlinkSync(sourceFile);
    console.info('Generated translation file ' + localeCode + '.json');
}

function flatten(o) {
    const prefix = arguments[1] || "", out = arguments[2] || {};
    for (const name in o) {
        if (o.hasOwnProperty(name)) {
            typeof o[name] === "object" ? flatten(o[name], prefix + name + '.', out) :
                out[prefix + name] = o[name];
        }
    }

    return out;
}


export default class ColourUtils {
    static isDark(hexColour: string) {
        const color = (hexColour.charAt(0) === '#') ? hexColour.substring(1, 7) : hexColour;
        const r = parseInt(color.substring(0, 2), 16);
        const g = parseInt(color.substring(2, 4), 16);
        const b = parseInt(color.substring(4, 6), 16);

        return (((r * 0.299) + (g * 0.587) + (b * 0.114)) > 186);
    }

    static hexToRgbA(hex: string, opacity: number = 1) {
        let colour;
        if (/^#([A-Fa-f0-9]{3}){1,2}$/.test(hex)) {
            colour = hex.substring(1).split('');
            if (colour.length === 3) {
                colour = [colour[0], colour[0], colour[1], colour[1], colour[2], colour[2]];
            }
            colour = '0x' + colour.join('');
            // tslint:disable-next-line:no-bitwise
            return 'rgba(' + [(colour >> 16) & 255, (colour >> 8) & 255, colour & 255].join(',') + ', ' + opacity + ')';
        }
    }
}

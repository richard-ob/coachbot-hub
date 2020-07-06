
export default class StringUtils {
    static upperCase(value: string) {
        if (!value) {
            return '';
        }

        return value.toUpperCase();
    }
}

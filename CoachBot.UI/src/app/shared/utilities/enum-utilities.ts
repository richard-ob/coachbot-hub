export default class EnumUtils {
    static toArray(enumObject: any) {
        return Object.keys(enumObject)
            .filter(value => isNaN(Number(value)) === false)
            .map(key => enumObject[key]);
    }
}

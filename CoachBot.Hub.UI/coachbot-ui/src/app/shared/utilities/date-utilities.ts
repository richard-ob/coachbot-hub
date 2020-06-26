import { DatePipe } from '@angular/common';

export default class DateUtils {
    static getDateString(year: number, month: number, week: number, day: number) {
        const datePipe = new DatePipe('en-gb');

        if (day) {
            const date = new Date(year, month, day);
            return datePipe.transform(date, 'MMMM d');
        } else if (week) {
            const date = this.getDateOfWeek(week, year);
            return datePipe.transform(date, 'MMMM d');
        } else if (month) {
            const date = new Date(year, month, day || 1);
            return datePipe.transform(date, 'MMMM');
        } else {
            const date = new Date(year, 0, 1);
            return datePipe.transform(date, 'yyyy');
        }
    }

    static getDateOfWeek(week: number, year: number) {
        const date = new Date(year, 0, (1 + (week - 1) * 7));
        date.setDate(date.getDate() + (1 - date.getDay()));
        return date;
    }
}

import { OnInit, Component, Input, ViewEncapsulation } from '@angular/core';
import { MatchDayTotals } from '@pages/hub/shared/model/team-match-day-totals';
import { DatePipe } from '@angular/common';

const monthName = new Intl.DateTimeFormat('en-us', { month: 'short' });
const weekdayName = new Intl.DateTimeFormat('en-us', { weekday: 'short' });

@Component({
    selector: 'app-calendar-heatmap',
    templateUrl: './calendar-heatmap.component.html',
    styleUrls: ['./calendar-heatmap.component.scss'],
    encapsulation: ViewEncapsulation.None
})
export class CalendarHeatmapComponent implements OnInit {

    @Input() calendarEntries: MatchDayTotals[];
    chartData: any;
    colorScheme = {
        domain: ['#dedede', '#b3e5fc', '#81d4fa', '#4fc3f7', '#29b6f6', '#03a9f4', '#039be5', '#0288d1', '#0277bd', '#01579b']
    };

    ngOnInit() {
        console.log(this.calendarEntries);
        this.chartData = this.getCalendarData();
    }

    calendarAxisTickFormatting(mondayString: string) {
        const monday = mondayString.split('/');
        const month = +monday[1] - 1;
        const day = +monday[0];
        const year = +monday[2];
        const lastSunday = new Date(year, month, day - 1);
        const nextSunday = new Date(year, month, day + 6);
        return (lastSunday.getMonth() !== nextSunday.getMonth()) ? monthName.format(nextSunday) : '';
    }

    getCalendarData(): any[] {
        // today
        const now = new Date();
        const todaysDay = now.getDate();
        const thisDay = new Date(now.getFullYear(), now.getMonth(), todaysDay);

        // Monday
        const thisMonday = new Date(thisDay.getFullYear(), thisDay.getMonth(), todaysDay - thisDay.getDay() + 1);
        const thisMondayDay = thisMonday.getDate();
        const thisMondayYear = thisMonday.getFullYear();
        const thisMondayMonth = thisMonday.getMonth();

        // 52 weeks before monday
        const calendarData = [];
        const getDate = d => new Date(thisMondayYear, thisMondayMonth, d);
        for (let week = -52; week <= 0; week++) {
            const mondayDay = thisMondayDay + (week * 7);
            const monday = getDate(mondayDay);

            // one week
            const series = [];
            for (let dayOfWeek = 7; dayOfWeek > 0; dayOfWeek--) {
                const date = getDate(mondayDay - 1 + dayOfWeek);

                // skip future dates
                if (date > now) {
                    continue;
                }

                // value
                const totals = this.calendarEntries.find(t => new Date(t.matchDayDate).getTime() === date.getTime());
                const value = totals ? totals.matches : 0;
                series.push({
                    date,
                    name: weekdayName.format(date),
                    value
                });
            }
            const datePipe = new DatePipe('en-gb');
            calendarData.push({
                name: datePipe.transform(monday, 'dd/MM/yyyy'),
                series
            });
        }

        return calendarData;
    }

}

import {WeatherUnit} from "./WeatherUnit";

export class WeatherOfDay {
  weatherUnits: WeatherUnit[];

  constructor(weatherUnits: WeatherUnit[]) {
    this.weatherUnits = weatherUnits;
  }

  getWeekDay(): string {
    const weekDay = ['Понедельник', 'Вторник', 'Среда', 'Четверг', 'Пятница', 'Суббота', 'Воскресение']
    const date = new Date(this.weatherUnits[0].date);
    return weekDay[date.getDay()];
  }

  getAverageTemp(): number {
    const temperatures = this.weatherUnits.map((weather) => weather.temperature);
    const sum = temperatures.reduce((acc, curr) => acc + curr, 0);
    return sum / temperatures.length;
  }

}

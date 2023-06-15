export interface WeatherUnit {
  date: string;
  time: string;
  temperature: number;
  humidity: number;
  dewPoint: number;
  pressure: number;
  windSpeed?: number;
  cloudCoverPercentage?: number;
  cloudBase?: number;
  visibility?: number;
  weatherType?: string;
  windDirections: WindDirection[];
}

export interface WindDirection {
  windDirectionId: number;
  name: string;
}


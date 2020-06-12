import { DataStatus } from './dataStatus';
import { WeatherForecast } from '../weatherForecast';

export interface WeatherState {
  dataStatus: DataStatus;
  data?: WeatherForecast[];
  error?: string;
  busy: boolean;
}

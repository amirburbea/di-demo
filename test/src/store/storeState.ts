import { PreferencesState } from './preferencesState';
import { WeatherState } from './weatherState';

export interface StoreState {
  preferences: PreferencesState;
  weather: WeatherState;
}

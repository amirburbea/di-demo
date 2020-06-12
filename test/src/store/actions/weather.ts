import { WeatherForecast } from '../../weatherForecast';
import { createAction } from 'redux-actions';
import { DataStatus } from '../dataStatus';

export const setData = createAction<WeatherForecast[] | undefined>(
  'WEATHER_SET_DATA'
);
export const setError = createAction<string | undefined>('WEATHER_SET_ERROR');
export const setDataStatus = createAction<DataStatus>('WEATHER_SET_STATUS');

export const setupDatabase = createAction('WEATHER_SETUP_DB', () => {});
export const tearDownDatabase = createAction('WEATHER_TEARDOWN_DB', () => {});
export const bulkInsertDatabase = createAction(
  'WEATHER_BULK_INSERT_DB',
  () => {}
);

export const setBusy = createAction<boolean>('WEATHER_SET_BUSY');

export const getData = createAction('WEATHER_GET_DATA', () => {});

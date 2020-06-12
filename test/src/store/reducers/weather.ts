import { WeatherState } from '../weatherState';
import { DataStatus } from '../dataStatus';
import { handleActions, Action } from 'redux-actions';
import { WeatherForecast } from '../../weatherForecast';
import updateImmutable from '../updateImmutable';

const initial: WeatherState = {
  dataStatus: 0 as DataStatus,
  busy: false,
};

export default handleActions<WeatherState, any>(
  {
    ['WEATHER_SET_BUSY']: (state, { payload }: Action<boolean>) => {
      return updateImmutable(state, 'busy', payload);
    },
    ['WEATHER_SET_ERROR']: (state, { payload: error }: Action<string>) => ({
      ...state,
      error,
      data: undefined,
    }),
    ['WEATHER_SET_DATA']: (
      state,
      { payload: data }: Action<WeatherForecast[]>
    ) => ({ ...state, data, error: undefined }),
    ['WEATHER_SET_STATUS']: (
      state,
      { payload: dataStatus }: Action<DataStatus>
    ) => ({ ...state, dataStatus, data: undefined, error: undefined }),
  },
  initial
);

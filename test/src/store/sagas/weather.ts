import { call, put, takeEvery } from 'redux-saga/effects';
import { WeatherForecast } from '../../weatherForecast';
import { setData, setDataStatus, setError, setBusy } from '../actions/weather';
import { DataStatus } from '../dataStatus';

export default function* () {
  yield takeEvery('WEATHER_SETUP_DB', onSetupDb);
  yield takeEvery('WEATHER_TEARDOWN_DB', onTeardown);
  yield takeEvery('WEATHER_BULK_INSERT_DB', onBulkInsert);
  yield takeEvery('WEATHER_GET_DATA', onGetData);
}

function* onSetupDb() {
  yield makeRequest('setup', 'POST', function* () {
    yield put(setDataStatus(DataStatus.notLoaded));
  });
}

function* onTeardown() {
  yield makeRequest('tearDown', 'POST', function* () {
    yield put(setDataStatus(DataStatus.tornDown));
  });
}

function* onBulkInsert() {
  yield makeRequest('bulkInsert', 'POST', function* () {
    yield put(setDataStatus(DataStatus.loaded));
  });
}

function* onGetData() {
  yield makeRequest('data', 'GET', function* (data: WeatherForecast[]) {
    yield put(setData(data));
  });
}

function* makeRequest(
  suffix: string,
  method: 'GET' | 'POST',
  onSuccess: (item?: any) => void
) {
  async function callService() {
    const response = await window.fetch(`api/weatherForecast/${suffix}`, {
      method,
    });
    if (response.status === 204) {
      return;
    }
    return response.json();
  }

  try {
    yield put(setBusy(true));
    const result = yield call(callService);
    yield call(onSuccess, result);
  } catch (error) {
    yield put(setError((error as Object).toString()));
  } finally {
    yield put(setBusy(false));
  }
}

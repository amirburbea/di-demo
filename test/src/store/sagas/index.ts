import preferencesSaga from './preferences';
import weatherSaga from './weather';
import { all, call } from 'redux-saga/effects';

export default function* () {
  yield all([call(preferencesSaga), call(weatherSaga)]);
}

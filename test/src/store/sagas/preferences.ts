import { takeEvery, call, put } from 'redux-saga/effects';
import { PreferencesState } from '../preferencesState';
import { setPreferences } from '../actions/preferences';
import { Action } from 'redux-actions';
import { ThemeName } from '../../themeName';

export default function* () {
  const preferences = yield call(getCurrentPreferences);
  if (preferences) {
    yield put(setPreferences(preferences));
  }
  yield takeEvery('PREFERENCES_SET_THEME', onSetThemeName);
}

function* onSetThemeName({ payload }: Action<ThemeName>) {
  yield call(savePreferences, { themeName: payload });
}

async function savePreferences(payload: PreferencesState) {
  const response = await window.fetch('api/preferences', {
    headers: { 'Content-Type': 'application/json' },
    method: 'POST',
    body: JSON.stringify(payload),
  });
  if (response.status > 204) {
    console.error(await response.text());
  }
}

async function getCurrentPreferences(): Promise<PreferencesState | null> {
  try {
    const response = await window.fetch('api/preferences');
    if (response.status == 200) {
      return await response.json();
    }
  } catch {}
  return null;
}

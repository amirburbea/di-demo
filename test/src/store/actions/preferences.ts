import { createAction } from 'redux-actions';
import { ThemeName } from '../../themeName';
import { PreferencesState } from '../preferencesState';

//export const setTheme = createAction(
//  'PREFERENCES_SET_THEME',
//  (themeName: ThemeName) => themeName
//);

export const setTheme = (themeName: ThemeName) => ({
  type: 'PREFERENCES_SET_THEME',
  payload: themeName,
});

export const setPreferences = createAction<PreferencesState>('PREFERENCES_SET');

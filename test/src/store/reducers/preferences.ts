import createBasicReducer from '../createBasicReducer';
import { PreferencesState } from '../preferencesState';
import { Action } from 'redux-actions';

const initialState: PreferencesState = { themeName: 'dark' };

export default createBasicReducer(
  initialState,
  {
    ['PREFERENCES_SET_THEME']: 'themeName',
  },
  {
    ['PREFERENCES_SET']: (_, { payload }: Action<PreferencesState>) => payload,
  }
);

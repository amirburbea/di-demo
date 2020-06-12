import { handleActions, Reducer, ReducerMap } from 'redux-actions';
import { Dictionary } from '../dictionary';
import updateImmutable from './updateImmutable';

/**
 * A large number of redux reducers simply take a set of actions in which the `payload` is
 * used to update an associated property.
 *
 * Given an initial state, a dictionary mapping action types to property names,
 * (and optionally a dictionary of other reducer methods compatible with `require('redux-actions').handleActions`),
 * this method returns a basic reducer which updates those properties.
 * @param initialState An initial state for the store.
 * @param propertyMap A dictionary mapping action types to property names.
 * @param partial Optionally, a dictionary of other reducer methods compatible with `require('redux-actions').handleActions`
 */
export default function createBasicReducer<State extends object>(
  initialState: State,
  propertyMap: Dictionary<keyof State>,
  partial: ReducerMap<State, any> = {}
): Reducer<State, any> {
  return handleActions<State, any>(
    Object.entries(propertyMap).reduce<ReducerMap<State, any>>(
      (output, [actionType, key]) => {
        output[actionType] = (state, { payload }) => {
          return updateImmutable(state, key, payload);
        };
        return output;
      },
      partial
    ),
    initialState
  );
}

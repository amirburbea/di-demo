/**
 * Function which takes as arguments an immutable `state`, a `property` to update and a
 * `value`. If the immutable state object has its `property` value equal to the supplied
 * `value`, it is returned (unchanged).  Otherwise, a clone of the original object is
 * returned, identical to the original `state`, with the exception that it has the supplied
 * `value` for the `property`.
 *
 * @param state An immutable state object.
 * @param property A property to update.
 * @param value A value to set for the property.
 * @param equals Optional, a function used to determine value equality.
 * @returns Either the original immutable `state`, or a clone with the `property`
 * set to the supplied `value`.
 */
export default function <State, Property extends keyof State>(
  state: State,
  property: Property,
  value: State[Property]
) {
  return state[property] === value ? state : { ...state, [property]: value };
}

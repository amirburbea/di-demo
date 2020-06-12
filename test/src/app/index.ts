import { App, AppProps } from './component';
import { MapStateToProps, connect } from 'react-redux';
import { StoreState } from '../store/storeState';
import { setTheme } from '../store/actions/preferences';
import { createSelector } from 'reselect';

type StateProps = Pick<AppProps, 'themeName' | 'dataStatus'>;
type OwnProps = {};

const mapStateToProps: MapStateToProps<
  StateProps,
  OwnProps,
  StoreState
> = createSelector(
  ({ preferences: { themeName } }: StoreState) => themeName,
  ({ weather }) => weather.dataStatus,
  (themeName, dataStatus) => ({ themeName, dataStatus })
);

type DispatchProps = Omit<AppProps, keyof (StateProps & OwnProps)>;

const dispatchProps: DispatchProps = { setTheme };

export default connect(mapStateToProps, dispatchProps)(App);

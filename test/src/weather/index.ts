import { Weather, WeatherProps } from './component';
import { MapStateToProps, connect } from 'react-redux';
import { StoreState } from '../store/storeState';
import {
  setupDatabase,
  tearDownDatabase,
  bulkInsertDatabase,
  getData,
} from '../store/actions/weather';

type StateProps = Pick<WeatherProps, 'data' | 'dataStatus' | 'error' | 'busy'>;
type OwnProps = {};

const mapStateToProps: MapStateToProps<StateProps, OwnProps, StoreState> = ({
  weather,
}) => weather;

type DispatchProps = Omit<WeatherProps, keyof (StateProps & OwnProps)>;

const dispatchProps: DispatchProps = {
  setupDatabase,
  tearDownDatabase,
  bulkInsertDatabase,
  requestData: getData,
};

export default connect(mapStateToProps, dispatchProps)(Weather);

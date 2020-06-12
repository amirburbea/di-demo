import React, { FunctionComponent } from 'react';
import styles from './styles.scss';
import { DataStatus } from '../store/dataStatus';
import { WeatherForecast } from '../weatherForecast';
import classNames from 'classnames';
import { Button } from '@blueprintjs/core';

export interface WeatherProps {
  dataStatus: DataStatus;
  data?: WeatherForecast[];
  error?: string;
  busy: boolean;
  setupDatabase: () => void;
  tearDownDatabase: () => void;
  bulkInsertDatabase: () => void;
  requestData: () => void;
}

export const Weather: FunctionComponent<WeatherProps> = ({
  busy,
  data,
  error,
  setupDatabase: setup,
  bulkInsertDatabase: bulkInsertData,
  requestData: getData,
  tearDownDatabase: tearDown,
}) => {
  return (
    <div className={classNames(styles.container, { [styles.busy]: busy })}>
      <Button onClick={setup}>SETUP</Button>
      <Button onClick={bulkInsertData}>BULK INSERT DATA</Button>
      <Button onClick={getData}>GET DATA</Button>
      <Button onClick={tearDown}>TEARDOWN</Button>

      <div style={{ overflow: 'scroll', maxWidth: 350, maxHeight: 300 }}>
        <table>
          <tbody>
            {data &&
              data.map((row, index) => (
                <tr key={index}>
                  <td>{JSON.stringify(row)}</td>
                </tr>
              ))}
            {error}
          </tbody>
        </table>
      </div>
    </div>
  );
};
